using NUnit.Framework;
using SheetLearner.Music.ViewModels;
using System.Collections.Generic;
using System.Linq;
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
            //Lower Treble Ledgers
            // F = 3 G = 2 
            // A = 2 B = 1
            // C = 1 D = 0
            Assert.AreEqual(3,NotesFactory.NumberOfLedgerLines(NotesFactory.F1,Clef.Treble)); 
            Assert.AreEqual(2,NotesFactory.NumberOfLedgerLines(NotesFactory.G1,Clef.Treble)); 
            Assert.AreEqual(2,NotesFactory.NumberOfLedgerLines(NotesFactory.A1,Clef.Treble)); 
            Assert.AreEqual(1,NotesFactory.NumberOfLedgerLines(NotesFactory.B1,Clef.Treble)); 
            Assert.AreEqual(1,NotesFactory.NumberOfLedgerLines(NotesFactory.C1,Clef.Treble)); 
            Assert.AreEqual(0,NotesFactory.NumberOfLedgerLines(NotesFactory.D1,Clef.Treble)); 

            //Higher Treble Ledgers
            // A = 1 B = 1
            // C = 2 D = 2 
            // E = 3
            Assert.AreEqual(1,NotesFactory.NumberOfLedgerLines(NotesFactory.A3,Clef.Treble)); 
            Assert.AreEqual(1,NotesFactory.NumberOfLedgerLines(NotesFactory.B3,Clef.Treble)); 
            Assert.AreEqual(2,NotesFactory.NumberOfLedgerLines(NotesFactory.C3,Clef.Treble)); 
            Assert.AreEqual(2,NotesFactory.NumberOfLedgerLines(NotesFactory.D3,Clef.Treble)); 
            Assert.AreEqual(3,NotesFactory.NumberOfLedgerLines(NotesFactory.E3,Clef.Treble));
        }

        [Test]
        public void NoteLedgerCountIsCorrectForBass()
        { 
            //Lower Bass Ledgers
            // A = 3 B = 2
            // C = 2 D = 1
            // E = 1 F = 0
            Assert.AreEqual(3,NotesFactory.NumberOfLedgerLines(NotesFactory.A1,Clef.Bass)); 
            Assert.AreEqual(2,NotesFactory.NumberOfLedgerLines(NotesFactory.B1,Clef.Bass)); 
            Assert.AreEqual(2,NotesFactory.NumberOfLedgerLines(NotesFactory.C1,Clef.Bass)); 
            Assert.AreEqual(1,NotesFactory.NumberOfLedgerLines(NotesFactory.D1,Clef.Bass)); 
            Assert.AreEqual(1,NotesFactory.NumberOfLedgerLines(NotesFactory.E1,Clef.Bass));
            Assert.AreEqual(0,NotesFactory.NumberOfLedgerLines(NotesFactory.F1,Clef.Bass));

            //Higher Treble Ledgers
            // B = 0 C = 1
            // D = 1 E = 2
            // F = 2 G = 3
            Assert.AreEqual(0,NotesFactory.NumberOfLedgerLines(NotesFactory.B3,Clef.Bass)); 
            Assert.AreEqual(1,NotesFactory.NumberOfLedgerLines(NotesFactory.C3,Clef.Bass)); 
            Assert.AreEqual(1,NotesFactory.NumberOfLedgerLines(NotesFactory.D3,Clef.Bass)); 
            Assert.AreEqual(2,NotesFactory.NumberOfLedgerLines(NotesFactory.E3,Clef.Bass));
            Assert.AreEqual(2,NotesFactory.NumberOfLedgerLines(NotesFactory.F3,Clef.Bass));
            Assert.AreEqual(3,NotesFactory.NumberOfLedgerLines(NotesFactory.G3,Clef.Bass));
        }



        [Test]
        public void GetsNoteInLedger()
        {
            var notes = NotesFactory.GetNotesInLedger(NotesFactory.C1, Clef.Bass);
            Assert.AreEqual(2, notes.Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(1, NotesFactory.GetNotesInLedger(NotesFactory.E1, Clef.Bass).Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(3, NotesFactory.GetNotesInLedger(NotesFactory.A1, Clef.Bass).Where((x,i) => i%2 == 0).ToList().Count);



            Assert.AreEqual(1, NotesFactory.GetNotesInLedger(NotesFactory.C3, Clef.Bass).Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(1, NotesFactory.GetNotesInLedger(NotesFactory.D3, Clef.Bass).Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(2, NotesFactory.GetNotesInLedger(NotesFactory.E3, Clef.Bass).Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(2, NotesFactory.GetNotesInLedger(NotesFactory.F3, Clef.Bass).Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(3, NotesFactory.GetNotesInLedger(NotesFactory.G3, Clef.Bass).Where((x,i) => i%2 == 0).ToList().Count);



            //Trebble tests
            Assert.AreEqual(1, NotesFactory.GetNotesInLedger(NotesFactory.C1, Clef.Treble).Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(2, NotesFactory.GetNotesInLedger(NotesFactory.A1, Clef.Treble).Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(2, NotesFactory.GetNotesInLedger(NotesFactory.G1, Clef.Treble).Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(3, NotesFactory.GetNotesInLedger(NotesFactory.F1, Clef.Treble).Where((x,i) => i%2 == 0).ToList().Count); 
        }

        [Test]
        public void NotesRelation()
        {
            var midC = NotesFactory.C2;
            var noteAbove = NotesFactory.C3;

            Assert.AreEqual(Relation.Lower, midC.RelationTo(noteAbove, Clef.Treble));
            Assert.AreEqual(Relation.Lower ,NotesFactory.D2.RelationTo(NotesFactory.E2, Clef.Bass));
            Assert.AreEqual(Relation.Higher, NotesFactory.C2.RelationTo(NotesFactory.C1, Clef.Treble));

            Assert.AreEqual(Relation.Equal, NotesFactory.D1.RelationTo(NotesFactory.D1, Clef.Treble));
        }

        [Test]
        public void NotesRelationToMid()
        {
            Assert.AreEqual(NotesFactory.E1.RelationToMidpoint(Clef.Bass), Relation.Lower); 
            Assert.AreEqual(NotesFactory.C3.RelationToMidpoint(Clef.Bass), Relation.Higher); 

            Assert.AreEqual(NotesFactory.C1.RelationToMidpoint(Clef.Treble), Relation.Lower);
            Assert.AreEqual(NotesFactory.C2.RelationToMidpoint(Clef.Treble), Relation.Higher);
        }
    } 
}