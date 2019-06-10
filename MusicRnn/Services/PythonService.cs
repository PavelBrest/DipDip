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
        private Task _task;
        private Process _process;

        public List<MidiFile> Result { get; private set; }

        public event EventHandler EpochResult;

        public PythonService()
        {
            Result = new List<MidiFile>();

            _process = new Process();
            _process.StartInfo = new ProcessStartInfo($@"{AppDomain.CurrentDomain.BaseDirectory}script\Start.bat")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = $@"{AppDomain.CurrentDomain.BaseDirectory}script"
            };
        }

        public void StartScript()
        {
            if (_task != null)
                _task.Dispose();

            _task = new Task(Start);
            
            _task.Start();
        }

        private void Start()
        {
            _process.Start();

            int count = 0;
            
            while(true)
            {
                var files = Directory.GetFiles($@"{AppDomain.CurrentDomain.BaseDirectory}script\output");
                
                if (count != files.Length)
                {
                    count = files.Length;
                    EpochResult?.Invoke(this, null);
                }
                Thread.Sleep(TimeSpan.FromMinutes(5));
            }
        }
    }

    internal interface IPythonService
    {
        event EventHandler EpochResult;
        
        List<MidiFile> Result { get; }
        void StartScript();
    }
}
