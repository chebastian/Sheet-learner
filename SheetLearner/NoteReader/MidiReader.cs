﻿using Midi;
using NoteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteReader
{
    //public class MidiReader : IMidiRepository
    //{
    //    private INoteListener _listener; 
    //    private InputDevice SelectedDevice { get; set; }
    //    private List<int> NotesPressed;

    //    public MidiReader(INoteListener listener, IMidiPublisher midi)
    //    {
    //        _listener = listener;
    //        midi.MidiDeviceChanged += Midi_MidiDeviceChanged;
    //    }

    //    private void Midi_MidiDeviceChanged(object sender, EventArgs e)
    //    {
    //        if(e is MidiListenerEventArgs arg)
    //        {
    //            SelectDeviceWithName(arg.SelectedDevice);
    //        }
    //    }
 
    //    public void SelectDefaultDevice()
    //    {
    //        SelectDeviceWithName(AvailableDevices.First()); 
    //    }

    //    public List<String> AvailableDevices
    //    {
    //        set { }
    //        get
    //        {
    //            var res = new List<string>();

    //            var number = NAudio.Midi.MidiIn.NumberOfDevices;

    //            for (var i = 0; i < number; i++)
    //            {
    //                var device = NAudio.Midi.MidiIn.DeviceInfo(i);
    //                res.Add(device.ProductName);
    //            }

    //            return res;
    //        }

    //    }

    //    private bool SelectDeviceWithName(String deviceName)
    //    {
    //        var devices = InputDevice.InstalledDevices.Any(X => X.Name == deviceName);
    //        if(!devices) 
    //            return false;

    //        var device = InputDevice.InstalledDevices.First(x => deviceName.Equals(x.Name));
    //        if (SelectedDevice != null)
    //        {
    //            SelectedDevice.NoteOn -= _callback;
    //        }

    //        SelectedDevice = device;
    //        if(!device.IsOpen)
    //            device.Open();
    //        device.NoteOn += _callback;
    //        device.NoteOff += OnNoteOffCallback;
    //        if(!device.IsReceiving)
    //            device.StartReceiving(null); 

    //        return true; 
    //    }

    //    private void OnNoteOffCallback(NoteOffMessage msg)
    //    {
    //        if (NotesPressed.Contains(msg.Pitch.PositionInOctave()))
    //            NotesPressed.Remove(msg.Pitch.PositionInOctave());
    //    }

    //    private void _callback(NoteOnMessage msg)
    //    {
    //        var test = msg.Pitch.NoteWithLetter('C');

    //        var idx = msg.Pitch.PositionInOctave();
    //        NotesPressed.Add(idx);

    //        if (NotesPressed.Count > 1) 
    //            _listener.OnNotesPressed(NotesPressed);
    //        else
    //            _listener.OnNotePressed(idx);
    //    }

    //    void IMidiRepository.SelectDeviceWithName(string name)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IMidiPublisher GetCurrentPublisher()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
