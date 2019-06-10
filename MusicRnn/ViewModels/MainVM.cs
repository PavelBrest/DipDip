using Microsoft.Win32;

using MusicRnn.Models;
using MusicRnn.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace MusicRnn.ViewModels
{
    class MainVM : BaseVM
    {
        private List<MidiFile> _midiFiles;
        private IPythonService _service;
        private SoundPlayer _player;
        private bool _isRunning;

        public event EventHandler UpdateView;
        public event EventHandler OutputUpdate;

        public IDelegateCommand LoadFilesCommand { get; private set; }
        public IDelegateCommand StartCommand { get; private set; }
        public IDelegateCommand PlayCommand { get; private set; }

        public IEnumerable<string> FilesNames { get => _midiFiles?.Select(p => p.Name); }
        public List<MidiFile> ResultFiles { get; set; }
        public Visibility ProgressVis { get; set; } = Visibility.Hidden;

        public string ButtonText { get; set; } = "Start";

        public bool IsRunnig
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                ButtonText = _isRunning ? "Stop" : "Start";

                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonText));
            }
        }

        public MainVM()
        {
            LoadFilesCommand = new DelegateCommand(OnLoadFilesExecute);
            PlayCommand = new DelegateCommand(OnPlayExecute);
            StartCommand = new DelegateCommand(OnStartExecute, CanStartExecute);
            ResultFiles = new List<MidiFile>();
            _midiFiles = new List<MidiFile>();
            _service = new PythonService();
            _service.EpochResult += EpochResult;
            _service.ConsolePrint += (s, args) =>
            {
                OutputUpdate?.Invoke(s, args);
            };
        }

        private void OnPlayExecute(object obj)
        {
            if (obj is MidiFile file)
            {
                _player?.Stop();
                if (_player?.SoundLocation != file.FullPath)
                {
                    _player = new SoundPlayer(file.FullPath);
                    _player.Load();
                    _player.Play();
                }
                else
                {
                    _player?.Dispose();
                    _player = null;
                }
            }
        }

        private bool CanStartExecute(object arg)
        {
            return _midiFiles.Count != 0;
        }

        private void EpochResult(object sender, EventArgs e)
        {
            foreach (var item in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\script\\output"))
            {
                if (ResultFiles.Count(p => p.FullPath == item) == 0)
                {
                    ResultFiles.Add(new MidiFile
                    {
                        FullPath = item,
                        Name = Regex.Match(item, @"\\([\w\d\.]+)\.mid").Groups[1].Value
                    });
                }
            }

            OnPropertyChanged(nameof(ResultFiles));
            UpdateView?.Invoke(this, null);
        }

        private void OnStartExecute(object obj)
        {
            if (IsRunnig)
            {
                ProgressVis = Visibility.Hidden;
                IsRunnig = false;

                OnPropertyChanged(nameof(ProgressVis));
                OnPropertyChanged(nameof(IsRunnig));
                return;
            }

            _service.StartScript();

            ProgressVis = Visibility.Visible;
            IsRunnig = true;

            OnPropertyChanged(nameof(ProgressVis));
            OnPropertyChanged(nameof(IsRunnig));
        }

        private void OnLoadFilesExecute(object obj)
        {
            _midiFiles.Clear();
            var dialog = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Choose MIDI files.",
                Filter = "MIDI files (*.mid)|*.mid",
                RestoreDirectory = true
            };


            if (dialog.ShowDialog() == true && dialog.CheckFileExists)
            {
                foreach (var item in dialog.FileNames)
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "script\\music\\" + Regex.Match(item, @"\\([\w\d\._-]+)\.mid").Groups[1].Value + ".mid";
                    if (File.Exists(path))
                        File.Delete(path);
                    else
                        File.Copy(item, path);

                    _midiFiles.Add(new MidiFile
                    {
                        FullPath = item,
                        Name = Regex.Match(item, @"\\([\w\d\._-]+)\.mid").Groups[1].Value
                    });
                }
                
                OnPropertyChanged(nameof(FilesNames));
                StartCommand.RaisCanExecuteChanged();
            }
        }
    }
}
