using NUnit.Framework;
using SheetLearner.Music.ViewModels;
using System.Collections.Generic;
using XTestMan.Views.Music;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var sheet = new Sheet(Clef.Bass);
            var c1Val = sheet.GetNoteValueInClef(Clef.Bass, NotesFactory.C1);
            var f2Val = sheet.GetNoteValueInClef(Clef.Bass, NotesFactory.F2);

            var f2Sharp = NotesFactory.F2.Sharped();
            var valSharped = sheet.GetNoteValueInClef(Clef.Bass, f2Sharp);

            Assert.That(f2Val,Is.EqualTo(2),"ERror");
            Assert.That(f2Val, Is.EqualTo(valSharped), "Error when sharp");
        }

        [Test]
        public void NoteLedgerCountIsCorrect()
        {
            var cf = new ClefViewModel(Clef.Treble);
            var notes = new List<Note>()
            { 
                NotesFactory.C1
            };

            cf.AddNoteGroup(notes);
        }
    }
}