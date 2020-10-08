using Shouldly;
using System;
using System.Collections.Generic;
using TestP.Lib.Models;
using Xunit;
using Xunit.Abstractions;

namespace TestP.Lib.Test
{
    public class HandTest
    {
        public HandTest(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        public ITestOutputHelper OutputHelper { get; }


        [Fact]
        public void TestRoyalStraightFlush()
        {
            var hand = new Hand("SA CA HA HQ H10 HJ CJ C10 SK SQ SJ S10");

            hand.BestHandDescription.ShouldBe(Hand.RoyalFlush);
            Hand compareHand = new Hand("SA SK SQ SJ S10");
            hand.BestHand.ShouldBe(compareHand.BestHand);
        }


        [Fact]
        public void TestStraightFlush()
        {
            var hand = new Hand("S5 H5 C6 SA S3 S2 S4 H6 H3");

            var compareHand = new Hand("S5 S4 S3 S2 SA");
            hand.BestHand.ShouldBe(compareHand.BestHand);
            hand.BestHandDescription.ShouldBe(Hand.StraightFlush);
        }

        [Fact]
        public void TestFourOfAKind()
        {
            var hand = new Hand("SA CQ SQ H3 C3 HA S3 CA D3 DQ HK SK DK HQ CK DA");

            var compareHand = new Hand("SA HA CA DA CK"); // Last is top card
            hand.BestHand.ShouldBe(compareHand.BestHand);
            hand.BestHandDescription.ShouldBe(Hand.FourOfAKind);
        }

        [Fact]
        public void TestFullHouse()
        {
            var hand = new Hand("DQ S2 SK CA C2 D2 CK S4");

            var compareHand = new Hand("S2 C2 D2 SK CK");
            hand.BestHand.ShouldBe(compareHand.BestHand);
            hand.BestHandDescription.ShouldBe(Hand.FullHouse);
        }

        [Fact]
        public void TestFlush()
        {
            var hand = new Hand("SA CK S3 S10 CQ S4 CJ S2 S6");

            var compareHand = new Hand("SA S10 S6 S4 S3");
            hand.BestHand.ShouldBe(compareHand.BestHand);
            hand.BestHandDescription.ShouldBe(Hand.Flush);
        }

        [Fact]
        public void TestStraight()
        {
            var hand = new Hand("S9 SA D2 C5 D8 C5 DK SJ C10 SQ");

            hand.BestHandDescription.ShouldBe(Hand.Straight);
            Hand compareHand = new Hand("SA DK SQ SJ C10");
            hand.BestHand.ShouldBe(compareHand.BestHand);
        }

        [Fact]
        public void TestThreeOfAKind()
        {
            var hand = new Hand("SA CA DA C2 C6 DQ DJ");

            hand.BestHandDescription.ShouldBe(Hand.ThreeOfAKind);
            Hand compareHand = new Hand("SA CA DA DQ DJ");
            hand.BestHand.ShouldBe(compareHand.BestHand);
        }

        [Fact]
        public void Test()
        {
            var hand1 = new Hand("C2 S2 HA HK SQ");
            var hand2 = new Hand("SQ CQ HA HK DJ");

            OutputHelper.WriteLine(hand1.HandValue.ToString());
            OutputHelper.WriteLine(hand2.HandValue.ToString());
        }
    }
}
