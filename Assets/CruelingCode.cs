using System;
using System.Linq;
using System.Text.RegularExpressions;
using TheCode;
using UnityEngine;

using Rnd = UnityEngine.Random;

/// <summary>
/// On the Subject of The Cruel Code
/// Created by Livio and Marksam32
/// </summary>
public class CruelingCode : MonoBehaviour
{
    public KMAudio Audio;
    public KMBombModule Module;
    public KMBombInfo BombInfo;
    public KMBombInfo Bomb;
    public KMBombInfo kmbombinfo;
    public KMSelectable[] NumberButtons;
    public KMSelectable ButtonR;
    public KMSelectable ButtonS;
    public TextMesh Display;
    private int moduleNumber1;
    private int moduleNumber2;
    private int moduleNumber3;
    private int solution;
    private int _moduleId;
    private int DR1;
    private int DR2;
    private int DR3;
    private int SQ1;
    private int SQ2;
    private int SQ3;
    private int shownnum;
    private int nextnum = 2;
    private string DOW;
    private bool queryStrike;
    private bool bombTime = false;

    void Start()
    {
        _moduleId++;
        for (int i = 0; i < NumberButtons.Length; i++)
            NumberButtons[i].OnInteract = GetButtonPressHandler(i);
        ButtonR.OnInteract = BR;
        ButtonS.OnInteract = BS;
        moduleNumber1 = Rnd.Range(999, 10000);
        Display.text = moduleNumber1.ToString();
        moduleNumber2 = Rnd.Range(999, 10000);
        moduleNumber3 = Rnd.Range(999, 10000);
        LogMessage("First module number was {0}", moduleNumber1);
        LogMessage("Second module number was {0}", moduleNumber2);
        LogMessage("Third module number was {0}", moduleNumber3);
        DOW = DateTime.Now.DayOfWeek.ToString();
        SQ1 = (int)Math.Sqrt(moduleNumber1);
        SQ2 = (int)Math.Sqrt(moduleNumber2);
        SQ3 = (int)Math.Sqrt(moduleNumber3);
        DR1 = ((moduleNumber1 - 1) % 9) + 1;
        DR2 = ((moduleNumber2 - 1) % 9) + 1;
        DR3 = ((moduleNumber3 - 1) % 9) + 1;
        if (SQ1 == 35)
        {
            int sumDR = DR1 + DR2 + DR3;
            solution = ((sumDR - 1) % 9) + 1;
           // LogMessage("Solution is {0}", solution);
        }
        else if (DR3 == DR1)
        {
            solution = SQ3;
           // LogMessage("Solution is {0}", solution);
        }
        else if (SQ2 == SQ3)
        {
            int Sum12 = moduleNumber1 + moduleNumber2;
            solution = (int)Math.Sqrt(Sum12);
           // LogMessage("Solution is {0}", solution);
        }
        else if (Bomb.GetSolvableModuleNames().Contains("Mastermind Cruel") || Bomb.GetSolvableModuleNames().Contains("Cruel Piano Keys"))
        {
            bombTime = true;
           // LogMessage("Solution isn't constant.");
        }
        else if (DOW == "Monday" && Bomb.GetOnIndicators().Contains("BOB"))
        {
            solution = 4321;
            //LogMessage("Solution is 4321");
        }
        else if (Bomb.GetSolvableModuleNames().Count == 12)
        {
            solution = 19;
          //  LogMessage("Solution is 19");
        }
        else 
        {
            solution = ((int)SQ2 / SQ1);
        }
        if (solution == 0)
            solution = 1;
        if (bombTime == false)
            LogMessage("Solution is {0}", solution);
        else
            LogMessage("Solution is variable.");
    }
    private bool BS()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, ButtonS.transform);
        if (shownnum == solution)
        {
            LogMessage("Correct answer given. Module solved.");
            Display.text = "1234";
            Module.HandlePass();
        }
        else
        {

            Strike();

        }

        return false;
    }

    private bool BR()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, ButtonR.transform);
        if (nextnum == 2)
        {
            SetNum2();
            nextnum++;
        }
        else
        {
            if (nextnum == 3)
            {
                SetNum3();
                nextnum++;
            }
            else
            {
                if (nextnum == 4)
                {
                    queryStrike = true;
                    Strike();
                    nextnum = 2;
                }
            }

        }
        return false;
    }

    private KMSelectable.OnInteractHandler GetButtonPressHandler(int btn)
    {
        return delegate
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, NumberButtons[btn].transform);
            if ((shownnum <= 999))
            {
                shownnum = (shownnum * 10) + btn;
                SetTexts();
            }
            return false;
        };
    }

    private void SetTexts()
    {
        Display.text = shownnum.ToString();
    }
    private void SetNum2()
    {
        Display.text = moduleNumber2.ToString();
    }
    private void SetNum3()
    {
        Display.text = moduleNumber3.ToString();
    }


    void LogMessage(string message, params object[] parameters)
    {
        Debug.LogFormat("[The Cruel Code #{0}] {1}", _moduleId, string.Format(message, parameters));
    }
    private void Strike()
    {
        if (queryStrike == true)
            LogMessage("You pressed the Query button 3 times! Strike given and generating new numbers.");
        else
            LogMessage("Wrong answer given. I expected {0}, but you gave me {1}!", solution, shownnum);
        shownnum = 0;
        nextnum = 2;
        queryStrike = false;
        Module.HandleStrike();
        Start();


    }
    private void Update()
    {
        if (bombTime == true)
            solution = (int)Bomb.GetTime();

    }





#pragma warning disable 414
    private string TwitchHelpMessage = @"Submit the answer with “!{0} submit 1234”. Query the next number with !{1} query”.";
#pragma warning restore 414

    KMSelectable[] ProcessTwitchCommand(string command)
    {
        Match m;

        command = command.Trim().ToLowerInvariant();
        if (command == "query")
            return new[] { ButtonR };
        else if ((m = Regex.Match(command, @"^submit (\d+)$", RegexOptions.IgnoreCase)).Success)
            return m.Groups[1].Value.Select(ch => NumberButtons[ch - '0']).Concat(new[] { ButtonS }).ToArray();
        return null;
    }
}
