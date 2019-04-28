using NUnit.Framework;
using SheetLearner.Music;
using SheetLearner.Music.ViewModels;
using System.Collections.Generic;
using System.Linq;

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
            var c1Val = sheet.GetNoteValueInClef(Clef.Bass, Notes.C1);
            var f2Val = sheet.GetNoteValueInClef(Clef.Bass, Notes.F2);

            var f2Sharp = Notes.F2.Sharped();
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
            Assert.AreEqual(3,Notes.NumberOfLedgerLines(Notes.F1,Clef.Treble)); 
            Assert.AreEqual(2,Notes.NumberOfLedgerLines(Notes.G1,Clef.Treble)); 
            Assert.AreEqual(2,Notes.NumberOfLedgerLines(Notes.A1,Clef.Treble)); 
            Assert.AreEqual(1,Notes.NumberOfLedgerLines(Notes.B1,Clef.Treble)); 
            Assert.AreEqual(1,Notes.NumberOfLedgerLines(Notes.C1,Clef.Treble)); 
            Assert.AreEqual(0,Notes.NumberOfLedgerLines(Notes.D1,Clef.Treble)); 

            //Higher Treble Ledgers
            // A = 1 B = 1
            // C = 2 D = 2 
            // E = 3
            Assert.AreEqual(1,Notes.NumberOfLedgerLines(Notes.A3,Clef.Treble)); 
            Assert.AreEqual(1,Notes.NumberOfLedgerLines(Notes.B3,Clef.Treble)); 
            Assert.AreEqual(2,Notes.NumberOfLedgerLines(Notes.C3,Clef.Treble)); 
            Assert.AreEqual(2,Notes.NumberOfLedgerLines(Notes.D3,Clef.Treble)); 
            Assert.AreEqual(3,Notes.NumberOfLedgerLines(Notes.E3,Clef.Treble));
        }

        [Test]
        public void NoteLedgerCountIsCorrectForBass()
        { 
            //Lower Bass Ledgers
            // A = 3 B = 2
            // C = 2 D = 1
            // E = 1 F = 0
            Assert.AreEqual(3,Notes.NumberOfLedgerLines(Notes.A1,Clef.Bass)); 
            Assert.AreEqual(2,Notes.NumberOfLedgerLines(Notes.B1,Clef.Bass)); 
            Assert.AreEqual(2,Notes.NumberOfLedgerLines(Notes.C1,Clef.Bass)); 
            Assert.AreEqual(1,Notes.NumberOfLedgerLines(Notes.D1,Clef.Bass)); 
            Assert.AreEqual(1,Notes.NumberOfLedgerLines(Notes.E1,Clef.Bass));
            Assert.AreEqual(0,Notes.NumberOfLedgerLines(Notes.F1,Clef.Bass));

            //Higher Treble Ledgers
            // B = 0 C = 1
            // D = 1 E = 2
            // F = 2 G = 3
            Assert.AreEqual(0,Notes.NumberOfLedgerLines(Notes.B3,Clef.Bass)); 
            Assert.AreEqual(1,Notes.NumberOfLedgerLines(Notes.C3,Clef.Bass)); 
            Assert.AreEqual(1,Notes.NumberOfLedgerLines(Notes.D3,Clef.Bass)); 
            Assert.AreEqual(2,Notes.NumberOfLedgerLines(Notes.E3,Clef.Bass));
            Assert.AreEqual(2,Notes.NumberOfLedgerLines(Notes.F3,Clef.Bass));
            Assert.AreEqual(3,Notes.NumberOfLedgerLines(Notes.G3,Clef.Bass));
        }



        [Test]
        public void GetsNoteInLedger()
        {
            var notes = Notes.GetNotesInLedger(Notes.C1, Clef.Bass);
            Assert.AreEqual(2, notes.Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(1, Notes.GetNotesInLedger(Notes.E1, Clef.Bass).Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(3, Notes.GetNotesInLedger(Notes.A1, Clef.Bass).Where((x,i) => i%2 == 0).ToList().Count);



            Assert.AreEqual(1, Notes.GetNotesInLedger(Notes.C3, Clef.Bass).Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(1, Notes.GetNotesInLedger(Notes.D3, Clef.Bass).Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(2, Notes.GetNotesInLedger(Notes.E3, Clef.Bass).Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(2, Notes.GetNotesInLedger(Notes.F3, Clef.Bass).Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(3, Notes.GetNotesInLedger(Notes.G3, Clef.Bass).Where((x,i) => i%2 == 0).ToList().Count);



            //Trebble tests
            Assert.AreEqual(1, Notes.GetNotesInLedger(Notes.C1, Clef.Treble).Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(2, Notes.GetNotesInLedger(Notes.A1, Clef.Treble).Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(2, Notes.GetNotesInLedger(Notes.G1, Clef.Treble).Where((x,i) => i%2 == 0).ToList().Count);
            Assert.AreEqual(3, Notes.GetNotesInLedger(Notes.F1, Clef.Treble).Where((x,i) => i%2 == 0).ToList().Count); 
        }

        [Test]
        public void NotesRelation()
        {
            var midC = Notes.C2;
            var noteAbove = Notes.C3;

            Assert.AreEqual(Relation.Lower, midC.RelationTo(noteAbove, Clef.Treble));
            Assert.AreEqual(Relation.Lower ,Notes.D2.RelationTo(Notes.E2, Clef.Bass));
            Assert.AreEqual(Relation.Higher, Notes.C2.RelationTo(Notes.C1, Clef.Treble));

            Assert.AreEqual(Relation.Equal, Notes.D1.RelationTo(Notes.D1, Clef.Treble));
        }

        [Test]
        public void NotesRelationToMid()
        {
            Assert.AreEqual(Notes.E1.RelationToMidpoint(Clef.Bass), Relation.Lower); 
            Assert.AreEqual(Notes.C3.RelationToMidpoint(Clef.Bass), Relation.Higher); 

            Assert.AreEqual(Notes.C1.RelationToMidpoint(Clef.Treble), Relation.Lower);
            Assert.AreEqual(Notes.C2.RelationToMidpoint(Clef.Treble), Relation.Higher);
        }

		[Test]
		public void NotesOctave()
		{
				Notes.E1.OctaveUp(Clef.Bass);
		}
    } 
}