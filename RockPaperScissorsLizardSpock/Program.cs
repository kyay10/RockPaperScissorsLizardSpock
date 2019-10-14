﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RockPaperScissorsLizardSpock
{
    class Program
    {
        private static Player playerUser = new Player(), playerComputer = new Player(Winner.Computer);
        private static Random rGen = new Random();

        //The exit string for the while loop
        private const string EXIT_FLAG = "Exit";

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Rock Paper Scissors Lizard Spock.");
            Console.WriteLine("\n Please choose a game mode from the following or enter its respective number (0 or 1):\n" + GameMode.Regular.GetDescription() + "\n" + GameMode.LettsEdition.GetDescription());
            string strGameMode;
            while ((strGameMode = Console.ReadLine()).Trim().ToLower() != GameMode.Regular.GetDescription().ToLower() && strGameMode.Trim().ToLower() != GameMode.LettsEdition.GetDescription().ToLower())
            {
                Console.WriteLine("Please enter a correct game mode from above");
            }
            playerUser.GameMode = strGameMode.Trim().ToLower() == GameMode.Regular.GetDescription().ToLower() ? GameMode.Regular : GameMode.LettsEdition;
            playerComputer.GameMode = playerUser.GameMode;
            string strInput = string.Empty;
            while (strInput.Trim().ToLower() != EXIT_FLAG.Trim().ToLower())
            {
                Console.WriteLine("\nRound " + (playerUser.RoundsPlayed + 1) +
                    "\nPlease enter one of the following choices or their respective numbers (starting from 1) " +
                    "or enter \"" + EXIT_FLAG + "\" to exit out of the program: ");
                //Print out all the possible choices
                foreach (int choice in Enum.GetValues(typeof(Choice)))
                {
                    Choice curChoice = ConvertIntegerToChoice(choice);
                    if (curChoice == Choice.None)
                    {
                        continue;
                    }
                    Console.WriteLine(curChoice.ToString(playerUser.GameMode));
                }
                //Read the user input
                strInput = Console.ReadLine();
                //Exit if user wants to
                if (strInput.Trim().ToLower() == EXIT_FLAG.Trim().ToLower())
                {
                    continue;
                }

                //Get the user and computer choices and set them into their respective players
                Winner winner;
                if (int.TryParse(strInput, out int intInput))
                {
                    playerUser.Choice = ConvertIntegerToChoice(intInput);
                }
                else
                {
                    playerUser.Choice = ConvertStringToChoice(strInput, playerUser.GameMode);
                }
                //If the user entered an invalid choice, tell them so.
                if (playerUser.Choice == Choice.None)
                {
                    Console.WriteLine("\nPlease enter a valid choice");
                    continue;
                }
                playerComputer.Choice = ConvertIntegerToChoice(rGen.Next(1, Enum.GetValues(typeof(Choice)).Length));

                //Decide who the winner is
                if (playerUser.Choice == playerComputer.Choice)
                    winner = Winner.Draw;
                else if (playerUser.Choice.CanBeat(playerComputer.Choice))
                    winner = Winner.User;
                else
                    winner = Winner.Computer;

                //Increment counters and display winner and stats
                playerUser.OnWinnerAnnounced(winner);
                playerComputer.OnWinnerAnnounced(winner);
                DisplayWinner(winner);
                //Wait 1 second.
                Thread.Sleep(1000);
                DisplayStats();
                ResetChoices();
                //Wait 1 second.
                Thread.Sleep(1000);
            }
        }

        private static void ResetChoices()
        {
            playerUser.Choice = Choice.None;
            playerComputer.Choice = Choice.None;
        }

        private static void DisplayWinner(Winner winner)
        {
            //Display the winner, then reset for a new game
            Console.WriteLine("\n" + winner.GetDescription() + "\n");
        }

        private static void DisplayStats()
        {
            //Display the stats using an overridden ToString() method
            Console.WriteLine("\nUser Stats: \n" + playerUser.ToString() + "\n");
            Console.WriteLine("Computer Stats: \n" + playerComputer.ToString() + "\n");
        }

        private static Choice ConvertIntegerToChoice(int intChoice)
        {
            System.Collections.IEnumerator valuesEnumerator = Enum.GetValues(typeof(Choice)).GetEnumerator();
            System.Collections.IEnumerator namesEnumerator = Enum.GetNames(typeof(Choice)).GetEnumerator();

            //Loop over all possible values and names
            while (valuesEnumerator.MoveNext() && namesEnumerator.MoveNext())
            {
                if (intChoice == (int)valuesEnumerator.Current)
                {
                    return (Choice)Enum.Parse(typeof(Choice), (string)namesEnumerator.Current);
                }
            }
            return Choice.None;
        }

        private static Choice ConvertStringToChoice(string strChoice, GameMode gameMode)
        {
            if (gameMode == GameMode.Regular)
            {
                System.Collections.IEnumerator namesEnumerator = Enum.GetNames(typeof(Choice)).GetEnumerator();

                //Loop over all choice names
                while (namesEnumerator.MoveNext())
                {
                    if (strChoice.Trim().ToLower() == namesEnumerator.Current.ToString().ToLower())
                    {
                        return (Choice)Enum.Parse(typeof(Choice), (string)namesEnumerator.Current);
                    }
                }
                return Choice.None;
            }
            else
            {

                return ConvertIntegerToChoice(Array.IndexOf(EnumUtils.strAlternativeChoiceNames, strChoice));
            }
        }
    }
}
