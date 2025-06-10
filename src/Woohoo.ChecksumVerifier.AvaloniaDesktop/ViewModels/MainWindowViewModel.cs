// Copyright (c) Hugues Valois. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Woohoo.ChecksumVerifier.AvaloniaDesktop.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Woohoo.ChecksumVerifier.AvaloniaDesktop.Services;
using Woohoo.ChecksumVerifier.Core;
using Woohoo.Security.Cryptography;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IStorageService storageService;
    private readonly BackgroundWorker worker;

    private ChecksumFile? checksumFile;

    public MainWindowViewModel(IStorageService storageService)
    {
        this.storageService = storageService;

        this.worker = new BackgroundWorker();
        this.worker.DoWork += this.Worker_DoWork;
        this.worker.ProgressChanged += this.Worker_ProgressChanged;
        this.worker.RunWorkerCompleted += this.Worker_RunWorkerCompleted;
        this.worker.WorkerReportsProgress = true;
        this.worker.WorkerSupportsCancellation = true;
        this.storageService = storageService;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ScanCommand))]
    public partial string ChecksumFilePath { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string OutputMatches { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string OutputMismatches { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string OutputMissings { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(BrowseChecksumFileCommand))]
    [NotifyCanExecuteChangedFor(nameof(RemoveScanFolderCommand))]
    [NotifyCanExecuteChangedFor(nameof(AddScanFolderCommand))]
    [NotifyCanExecuteChangedFor(nameof(ScanCommand))]
    [NotifyCanExecuteChangedFor(nameof(CancelScanCommand))]
    public partial bool IsScanning { get; set; } = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CancelScanCommand))]
    public partial bool IsCancelling { get; set; } = false;

    [ObservableProperty]
    public partial int ProgressPercentage { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveScanFolderCommand))]
    public partial object? SelectedScanFolder { get; set; } = null;

    public ObservableCollection<HashResultViewModel> Results { get; set; } = [];

    public ObservableCollection<ScanFolderViewModel> ScanFolders { get; set; } = [];

    public bool ShowMenu { get; } = !RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    public bool ShowCommandBar { get; } = false; // !RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    private void LoadChecksumFile()
    {
        string ext = Path.GetExtension(this.ChecksumFilePath).ToLowerInvariant();
        switch (ext)
        {
            case ".md5":
                this.checksumFile = Md5FileSerializer.LoadFrom(this.ChecksumFilePath);
                break;

            case ".sfv":
                this.checksumFile = SfvFileSerializer.LoadFrom(this.ChecksumFilePath);
                break;

            default:
                throw new NotSupportedException(string.Format(Localized.ErrorFileExtNotSupported, ext));
        }

        this.OutputMatches = string.Empty;
        this.OutputMismatches = string.Empty;
        this.OutputMissings = string.Empty;
        this.Results.Clear();

        foreach (var entry in this.checksumFile.Entries)
        {
            HashResultViewModel hashResultViewModel = new();
            hashResultViewModel.Name = entry.Name;
            hashResultViewModel.Algorithm = entry.Algorithm;
            hashResultViewModel.Expected = entry.Checksum;

            this.Results.Add(hashResultViewModel);
        }
    }

    [RelayCommand(CanExecute = nameof(CanBrowseChecksumFile))]
    private async Task BrowseChecksumFileAsync()
    {
        var filePaths = await this.storageService.GetFilePathsAsync(Localized.BrowseChecksumFile, allowMultiple: false);
        if (filePaths.Any())
        {
            var filePath = filePaths.First();
            this.ChecksumFilePath = File.Exists(filePath) ? filePath : string.Empty;

            var folderPath = Path.GetDirectoryName(filePath);
            if (Directory.Exists(folderPath))
            {
                this.ScanFolders.Add(new ScanFolderViewModel() { FolderPath = folderPath });
            }
        }

        this.ScanCommand.NotifyCanExecuteChanged();
    }

    private bool CanBrowseChecksumFile()
    {
        return !this.IsScanning;
    }

    [RelayCommand(CanExecute = nameof(CanAddScanFolder))]
    private async Task AddScanFolderAsync()
    {
        var folderPaths = await this.storageService.GetFolderPathsAsync(Localized.BrowseScanFolder, allowMultiple: false);
        if (folderPaths.Any())
        {
            var folderPath = folderPaths.First();
            this.ScanFolders.Add(new ScanFolderViewModel() { FolderPath = folderPath });

            this.ScanCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanAddScanFolder()
    {
        return !this.IsScanning;
    }

    [RelayCommand(CanExecute = nameof(CanRemoveScanFolder))]
    private void RemoveScanFolder()
    {
        if (this.SelectedScanFolder is ScanFolderViewModel sfvm)
        {
            this.ScanFolders.Remove(sfvm);

            this.ScanCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanRemoveScanFolder()
    {
        return !this.IsScanning && this.SelectedScanFolder is ScanFolderViewModel;
    }

    [RelayCommand(CanExecute = nameof(CanScan))]
    private void Scan()
    {
        var scanFolderPaths = this.ScanFolders.Select(sf => sf.FolderPath).Where(fp => Directory.Exists(fp)).ToArray();
        if (!scanFolderPaths.Any())
        {
            return;
        }

        this.LoadChecksumFile();

        if (this.checksumFile is null)
        {
            return;
        }

        this.IsScanning = true;

        this.worker.RunWorkerAsync(new Tuple<ChecksumFile, string[]>(this.checksumFile, scanFolderPaths));
    }

    private bool CanScan()
    {
        if (this.IsScanning)
        {
            return false;
        }

        if (!File.Exists(this.ChecksumFilePath))
        {
            return false;
        }

        return this.ScanFolders.Any(sf => Directory.Exists(sf.FolderPath));
    }

    [RelayCommand(CanExecute = nameof(CanCancelScan))]
    private void CancelScan()
    {
        if (!this.IsScanning)
        {
            return;
        }

        this.IsCancelling = true;

        this.worker.CancelAsync();
    }

    private bool CanCancelScan()
    {
        return this.IsScanning && !this.IsCancelling;
    }

    private void Worker_DoWork(object? sender, DoWorkEventArgs e)
    {
        if (this.worker.CancellationPending)
        {
            e.Cancel = true;
            return;
        }

        if (e.Argument is null)
        {
            return;
        }

        var arg = (Tuple<ChecksumFile, string[]>)e.Argument;
        var file = arg.Item1;
        var scanFolderPaths = arg.Item2;

        for (int i = 0; i < file.Entries.Count; i++)
        {
            var entry = file.Entries[i];

            string actualHash = string.Empty;

            foreach (var folderPath in scanFolderPaths)
            {
                if (this.worker.CancellationPending)
                {
                    break;
                }

                var fullFilePath = Path.Combine(folderPath, entry.Name);
                if (File.Exists(fullFilePath))
                {
                    var calculator = new HashCalculator();
                    var calculatorResult = calculator.Calculate(new[] { entry.Algorithm }, fullFilePath);
                    actualHash = HashCalculator.HexToString(calculatorResult.Checksums[entry.Algorithm]);
                    break;
                }
            }

            if (this.worker.CancellationPending)
            {
                break;
            }

            var entryResult = new Tuple<ChecksumEntry, string>(entry, actualHash);
            this.worker.ReportProgress((int)((double)i / file.Entries.Count), entryResult);
        }
    }

    private void Worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
    {
        if (e.UserState is null)
        {
            return;
        }

        var result = (Tuple<ChecksumEntry, string>)e.UserState;
        var entry = result.Item1;
        var actualHash = result.Item2;

        var hashResultViewModel = this.Results.FirstOrDefault(r => r.Name == entry.Name);
        if (hashResultViewModel is not null)
        {
            hashResultViewModel.Actual = actualHash;
            if (actualHash.Length > 0)
            {
                if (string.Equals(actualHash, entry.Checksum, StringComparison.OrdinalIgnoreCase))
                {
                    hashResultViewModel.Status = Localized.ResultPass;
                    this.OutputMatches += entry.Name + Environment.NewLine;
                }
                else
                {
                    hashResultViewModel.Status = Localized.ResultFail;
                    this.OutputMismatches += entry.Name + Environment.NewLine;
                }
            }
            else
            {
                hashResultViewModel.Status = Localized.ResultMiss;
                this.OutputMissings += entry.Name + Environment.NewLine;
            }
        }

        this.ProgressPercentage = e.ProgressPercentage;
    }

    private void Worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        this.IsScanning = false;
        this.IsCancelling = false;

        foreach (var hashResultViewModel in this.Results)
        {
            if (hashResultViewModel.Actual.Length == 0)
            {
                hashResultViewModel.Actual = Localized.ResultActualNA;
                hashResultViewModel.Status = Localized.ResultCancel;
            }
        }
    }
}
