string datafolder = "Data/";
string usersfile = $"{datafolder}users.txt";
string resultsfile = $"{datafolder}results.txt";

Directory.CreateDirectory(datafolder);

Console.WriteLine("=== Quiz ===");

string currentlogin = "";
bool loggedin = false;

while (!loggedin)
{
    Console.WriteLine();
    Console.WriteLine("1 - Login");
    Console.WriteLine("2 - Register");
    Console.WriteLine("3 - Exit");
    Console.Write("Select an option: ");
    var choice = Console.ReadLine();

    if (choice == "1")
    {
        Console.Write("Username: ");
        var login = Console.ReadLine();
        Console.Write("Password: ");
        var password = Console.ReadLine();

        var found = false;
        if (File.Exists(usersfile))
        {
            var lines = File.ReadAllLines(usersfile);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length >= 2)
                {
                    if (parts[0] == login && parts[1] == password)
                    {
                        found = true;
                        break;
                    }
                }
            }
        }

        if (found)
        {
            currentlogin = login;
            loggedin = true;
            Console.WriteLine($"Welcome, {currentlogin}!");
        }
        else
        {
            Console.WriteLine("Invalid username or password.");
        }
    }
    else if (choice == "2")
    {
        Console.Write("Create username: ");
        var newlogin = Console.ReadLine();

        var taken = false;
        if (File.Exists(usersfile))
        {
            var existinglines = File.ReadAllLines(usersfile);
            foreach (var line in existinglines)
            {
                var parts = line.Split('|');
                if (parts[0] == newlogin)
                {
                    taken = true;
                    break;
                }
            }
        }

        if (taken)
        {
            Console.WriteLine("Username is already taken.");
        }
        else
        {
            Console.Write("Create password: ");
            var newpassword = Console.ReadLine();

            var dobok = false;
            var dob = "";
            while (!dobok)
            {
                Console.Write("Date of birth (dd.mm.yyyy): ");
                dob = Console.ReadLine();

                if (DateTime.TryParseExact(dob, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parseddate))
                {
                    if (parseddate.Year >= 1900 && parseddate.Year <= 2026)
                    {
                        dobok = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid year. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid format or nonexistent date. Use dd.mm.yyyy");
                }
            }

            var newline = $"{newlogin}|{newpassword}|{dob}";
            File.AppendAllText(usersfile, $"{newline}{Environment.NewLine}");
            Console.WriteLine("Registration successful. Please login now.");
        }
    }
    else if (choice == "3")
    {
        return;
    }
    else
    {
        Console.WriteLine("Invalid menu option.");
    }
}

var exit = false;
while (!exit)
{
    Console.WriteLine();
    Console.WriteLine("=== Main Menu ===");
    Console.WriteLine("1 - Start new quiz");
    Console.WriteLine("2 - My results");
    Console.WriteLine("3 - Top-20");
    Console.WriteLine("4 - Settings");
    Console.WriteLine("5 - Exit");
    Console.Write("Select an option: ");
    var menuchoice = Console.ReadLine();

    if (menuchoice == "1")
    {
        var categoryfiles = Directory.GetFiles(datafolder, "questions_*.txt");
        var categories = new string[categoryfiles.Length];

        for (int i = 0; i < categoryfiles.Length; i++)
        {
            var filename = Path.GetFileNameWithoutExtension(categoryfiles[i]);
            categories[i] = filename.Replace("questions_", "");
        }

        if (categories.Length == 0)
        {
            Console.WriteLine("No question categories available yet.");
        }
        else
        {
            Console.WriteLine("Select category:");
            for (int i = 0; i < categories.Length; i++)
            {
                Console.WriteLine($"{i + 1} - {categories[i]}");
            }
            Console.WriteLine($"{categories.Length + 1} - Mixed");
            Console.Write("Your choice: ");
            var catinput = Console.ReadLine();

            if (!int.TryParse(catinput, out int catchoice) || catchoice < 1 || catchoice > categories.Length + 1)
            {
                Console.WriteLine("Invalid choice.");
            }
            else
            {
                string categoryname;
                var questionlines = new List<string>();

                if (catchoice <= categories.Length)
                {
                    categoryname = categories[catchoice - 1];
                    var qfile = $"{datafolder}questions_{categoryname}.txt";
                    questionlines.AddRange(File.ReadAllLines(qfile));
                }
                else
                {
                    categoryname = "Mixed";
                    foreach (var cat in categories)
                    {
                        var qfile = $"{datafolder}questions_{cat}.txt";
                        questionlines.AddRange(File.ReadAllLines(qfile));
                    }
                }

                var rnd = new Random();
                for (int i = questionlines.Count - 1; i > 0; i--)
                {
                    var j = rnd.Next(i + 1);
                    var temp = questionlines[i];
                    questionlines[i] = questionlines[j];
                    questionlines[j] = temp;
                }

                var totalquestions = questionlines.Count < 20 ? questionlines.Count : 20;

                if (totalquestions == 0)
                {
                    Console.WriteLine("No questions in this category yet.");
                }
                else
                {
                    var correctcount = 0;

                    for (int q = 0; q < totalquestions; q++)
                    {
                        var parts = questionlines[q].Split('|');
                        var questiontext = parts[0];
                        var options = parts[1].Split(';');
                        var correctparts = parts[2].Split(',');

                        Console.WriteLine();
                        Console.WriteLine($"Question {q + 1} of {totalquestions}: {questiontext}");

                        for (int o = 0; o < options.Length; o++)
                        {
                            Console.WriteLine($"{o + 1}) {options[o]}");
                        }

                        Console.Write("Your answer (comma-separated numbers if multiple): ");
                        var answer = Console.ReadLine();
                        var userparts = answer.Split(',');

                        var useranswers = new List<int>();
                        foreach (var u in userparts)
                        {
                            if (int.TryParse(u.Trim(), out int val))
                            {
                                useranswers.Add(val);
                            }
                        }

                        var correctanswers = new List<int>();
                        foreach (var c in correctparts)
                        {
                            if (int.TryParse(c.Trim(), out int val))
                            {
                                correctanswers.Add(val);
                            }
                        }

                        var iscorrect = useranswers.Count == correctanswers.Count;
                        if (iscorrect)
                        {
                            foreach (var correctans in correctanswers)
                            {
                                if (!useranswers.Contains(correctans))
                                {
                                    iscorrect = false;
                                    break;
                                }
                            }
                        }

                        if (iscorrect)
                        {
                            correctcount++;
                            Console.WriteLine("Correct!");
                        }
                        else
                        {
                            Console.WriteLine("Incorrect.");
                        }
                    }

                    Console.WriteLine();
                    Console.WriteLine($"Quiz finished. Correct answers: {correctcount} out of {totalquestions}");

                    var resultline = $"{currentlogin}|{categoryname}|{correctcount}|{DateTime.Now:dd.MM.yyyy HH:mm}";
                    File.AppendAllText(resultsfile, $"{resultline}{Environment.NewLine}");

                    var allresults = File.ReadAllLines(resultsfile);
                    var scoresforcategory = new List<int>();

                    foreach (var res in allresults)
                    {
                        var rparts = res.Split('|');
                        if (rparts[1] == categoryname && int.TryParse(rparts[2], out int score))
                        {
                            scoresforcategory.Add(score);
                        }
                    }

                    scoresforcategory.Sort();
                    scoresforcategory.Reverse();

                    var place = scoresforcategory.IndexOf(correctcount) + 1;
                    Console.WriteLine($"Your rank in '{categoryname}' category: {place}");
                }
            }
        }
    }
    else if (menuchoice == "2")
    {
        Console.WriteLine();
        Console.WriteLine("=== My Results ===");

        var any = false;
        if (File.Exists(resultsfile))
        {
            var alllines = File.ReadAllLines(resultsfile);
            foreach (var line in alllines)
            {
                var parts = line.Split('|');
                if (parts[0] == currentlogin)
                {
                    any = true;
                    Console.WriteLine($"{parts[1]} - {parts[2]} correct - {parts[3]}");
                }
            }
        }

        if (!any)
        {
            Console.WriteLine("You have no quiz results yet.");
        }
    }
    else if (menuchoice == "3")
    {
        var categoryfiles = Directory.GetFiles(datafolder, "questions_*.txt");
        var categories = new string[categoryfiles.Length];

        for (int i = 0; i < categoryfiles.Length; i++)
        {
            var filename = Path.GetFileNameWithoutExtension(categoryfiles[i]);
            categories[i] = filename.Replace("questions_", "");
        }

        Console.WriteLine("Select category:");
        for (int i = 0; i < categories.Length; i++)
        {
            Console.WriteLine($"{i + 1} - {categories[i]}");
        }
        Console.WriteLine($"{categories.Length + 1} - Mixed");
        Console.Write("Your choice: ");
        var topinput = Console.ReadLine();

        if (!int.TryParse(topinput, out int topchoice) || topchoice < 1 || topchoice > categories.Length + 1)
        {
            Console.WriteLine("Invalid choice.");
        }
        else
        {
            var topcategory = topchoice <= categories.Length ? categories[topchoice - 1] : "Mixed";
            var filtered = new List<string>();

            if (File.Exists(resultsfile))
            {
                var alllines = File.ReadAllLines(resultsfile);
                foreach (var line in alllines)
                {
                    var parts = line.Split('|');
                    if (parts[1] == topcategory)
                    {
                        filtered.Add(line);
                    }
                }
            }

            filtered.Sort((x, y) =>
            {
                int scorex = int.Parse(x.Split('|')[2]);
                int scorey = int.Parse(y.Split('|')[2]);
                return scorey.CompareTo(scorex);
            });

            Console.WriteLine();
            Console.WriteLine($"=== Top-20: {topcategory} ===");

            var limit = filtered.Count < 20 ? filtered.Count : 20;
            if (limit == 0)
            {
                Console.WriteLine("No results yet.");
            }

            for (int i = 0; i < limit; i++)
            {
                var p = filtered[i].Split('|');
                Console.WriteLine($"{i + 1}. {p[0]} - {p[2]}");
            }
        }
    }
    else if (menuchoice == "4")
    {
        Console.WriteLine();
        Console.WriteLine("=== Settings ===");
        Console.WriteLine("1 - Change password");
        Console.WriteLine("2 - Change date of birth");
        Console.WriteLine("3 - Back");
        Console.Write("Select an option: ");
        var setchoice = Console.ReadLine();

        if (setchoice == "1")
        {
            Console.Write("New password: ");
            var newpass = Console.ReadLine();

            var alllines = File.ReadAllLines(usersfile);
            var newlines = new List<string>();

            foreach (var line in alllines)
            {
                var parts = line.Split('|');
                if (parts[0] == currentlogin)
                {
                    newlines.Add($"{parts[0]}|{newpass}|{parts[2]}");
                }
                else
                {
                    newlines.Add(line);
                }
            }
            File.WriteAllLines(usersfile, newlines);
            Console.WriteLine("Password changed.");
        }
        else if (setchoice == "2")
        {
            var dobok = false;
            var dob = "";
            while (!dobok)
            {
                Console.Write("New date of birth (dd.mm.yyyy): ");
                dob = Console.ReadLine();

                if (DateTime.TryParseExact(dob, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parseddate))
                {
                    if (parseddate.Year >= 1900 && parseddate.Year <= 2026)
                    {
                        dobok = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid year. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid format or nonexistent date. Use dd.mm.yyyy");
                }
            }

            var alllines = File.ReadAllLines(usersfile);
            var newlines = new List<string>();

            foreach (var line in alllines)
            {
                var parts = line.Split('|');
                if (parts[0] == currentlogin)
                {
                    newlines.Add($"{parts[0]}|{parts[1]}|{dob}");
                }
                else
                {
                    newlines.Add(line);
                }
            }
            File.WriteAllLines(usersfile, newlines);
            Console.WriteLine("Date of birth changed.");
        }
    }
    else if (menuchoice == "5")
    {
        exit = true;
        Console.WriteLine("Goodbye!");
    }
    else
    {
        Console.WriteLine("Invalid menu choice.");
    }
}