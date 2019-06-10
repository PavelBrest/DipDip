using MusicRnn.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MusicRnn.Services
{
    class PythonService : IPythonService
    {
        private bool _isRunning = false;
        private Task _task;
        private Process _process;

        public List<MidiFile> Result { get; private set; }

        public event EventHandler EpochResult;
        public event EventHandler ConsolePrint;

        public PythonService()
        {
            Result = new List<MidiFile>();

            _isRunning = false;
            _task = new Task(Start);

            _process = new Process();
            _process.StartInfo = new ProcessStartInfo($@"{AppDomain.CurrentDomain.BaseDirectory}script\Start.bat")
            {
                UseShellExecute = false,
                CreateNoWindow = false,
                WorkingDirectory = $@"{AppDomain.CurrentDomain.BaseDirectory}script",
                RedirectStandardOutput = true
            };
        }

        public void StartScript()
        {
            if (_isRunning)
            {
                _isRunning = false;
                _process.Close();
                return;
            }

            _isRunning = true;
            _task.Start();
        }

        private void Start()
        {
            _process.Start();

            int count = 0;
            
            while(_isRunning)
            {
                using (var reader = _process.StandardOutput)
                {
                    var files = Directory.GetFiles($@"{AppDomain.CurrentDomain.BaseDirectory}script\output");

                    while(!reader.EndOfStream)
                    {
                        ConsolePrint?.Invoke(reader.ReadLine(), null);
                    }


                    if (count != files.Length)
                    {
                        count = files.Length;
                        EpochResult?.Invoke(this, null);
                    }
                }
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }
    }

    internal interface IPythonService
    {
        event EventHandler EpochResult;
        event EventHandler ConsolePrint;

        List<MidiFile> Result { get; }
        void StartScript();
    }
}
