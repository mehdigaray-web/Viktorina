string DataFolder = "Data/";
string EditorUsersFile = DataFolder + "editor_users.txt";

Directory.CreateDirectory(DataFolder);
if (!File.Exists(EditorUsersFile))
{
    File.Create(EditorUsersFile).Close();
}

Console.WriteLine("=== Quiz Editor ===");

string CurrentLogin = "";
bool LoggedIn = false;

while (!LoggedIn)
{
    Console.WriteLine();
    Console.WriteLine("1 - Log in");
    Console.WriteLine("2 - Register");
    Console.WriteLine("3 - Exit");
    Console.Write("Select an option: ");
    string Choice = Console.ReadLine();

    if (Choice == "1")
    {
        Console.Write("Login: ");
        string Login = Console.ReadLine();
        Console.Write("Password: ");
        string Password = Console.ReadLine();

        string[] Lines = File.ReadAllLines(EditorUsersFile);
        bool Found = false;
        foreach (string Line in Lines)
        {
            string[] Parts = Line.Split('|');
            if (Parts[0] == Login && Parts[1] == Password)
            {
                Found = true;
            }
        }

        if (Found)
        {
            CurrentLogin = Login;
            LoggedIn = true;
            Console.WriteLine($"Welcome, {CurrentLogin}!");
        }
        else
        {
            Console.WriteLine("Invalid login or password.");
        }
    }
    else if (Choice == "2")
    {
        Console.Write("Enter new login: ");
        string NewLogin = Console.ReadLine();

        string[] ExistingLines = File.ReadAllLines(EditorUsersFile);
        bool Taken = false;
        foreach (string Line in ExistingLines)
        {
            string[] Parts = Line.Split('|');
            if (Parts[0] == NewLogin)
            {
                Taken = true;
            }
        }

        if (Taken)
        {
            Console.WriteLine("This login is already taken.");
        }
        else
        {
            Console.Write("Enter new password: ");
            string NewPassword = Console.ReadLine();
            File.AppendAllText(EditorUsersFile, $"{NewLogin}|{NewPassword}" + Environment.NewLine);
            Console.WriteLine("Registration successful. Please log in.");
        }
    }
    else if (Choice == "3")
    {
        return;
    }
    else
    {
        Console.WriteLine("Invalid menu option.");
    }
}

bool Exit = false;
while (!Exit)
{
    Console.WriteLine();
    Console.WriteLine("=== Editor Menu ===");
    Console.WriteLine("1 - Show category questions");
    Console.WriteLine("2 - Add question");
    Console.WriteLine("3 - Edit question");
    Console.WriteLine("4 - Delete question");
    Console.WriteLine("5 - Create new category");
    Console.WriteLine("6 - Exit");
    Console.Write("Select an option: ");
    string MenuChoice = Console.ReadLine();

    if (MenuChoice == "1" || MenuChoice == "2" || MenuChoice == "3" || MenuChoice == "4")
    {
        string[] CategoryFiles = Directory.GetFiles(DataFolder, "questions_*.txt");
        string[] Categories = new string[CategoryFiles.Length];

        for (int I = 0; I < CategoryFiles.Length; I++)
        {
            string FileName = Path.GetFileNameWithoutExtension(CategoryFiles[I]);
            Categories[I] = FileName.Replace("questions_", "");
        }

        if (Categories.Length == 0)
        {
            Console.WriteLine("No categories found. Create one first (option 5).");
        }
        else
        {
            Console.WriteLine("Categories:");
            for (int I = 0; I < Categories.Length; I++)
            {
                Console.WriteLine($"{I + 1} - {Categories[I]}");
            }
            Console.Write("Select category: ");
            string CatInput = Console.ReadLine();

            if (int.TryParse(CatInput, out int CatChoice) && CatChoice >= 1 && CatChoice <= Categories.Length)
            {
                string Category = Categories[CatChoice - 1];
                string QuestionFile = $"{DataFolder}questions_{Category}.txt";
                string[] QuestionLines = File.ReadAllLines(QuestionFile);

                if (MenuChoice == "1")
                {
                    if (QuestionLines.Length == 0)
                    {
                        Console.WriteLine("No questions in this category.");
                    }
                    else
                    {
                        for (int I = 0; I < QuestionLines.Length; I++)
                        {
                            string[] Parts = QuestionLines[I].Split('|');
                            Console.WriteLine($"{I + 1}. {Parts[0]} (options: {Parts[1]}, correct: {Parts[2]})");
                        }
                    }
                }
                else if (MenuChoice == "2")
                {
                    Console.Write("Question text: ");
                    string Text = Console.ReadLine();
                    Console.Write("How many options? ");
                    if (int.TryParse(Console.ReadLine(), out int OptionCount))
                    {
                        List<string> Options = new List<string>();
                        for (int I = 0; I < OptionCount; I++)
                        {
                            Console.Write($"Option {I + 1}: ");
                            Options.Add(Console.ReadLine());
                        }

                        Console.Write("Correct answer numbers (comma separated): ");
                        string Correct = Console.ReadLine();

                        string OptionsJoined = string.Join(";", Options);
                        string NewLine = $"{Text}|{OptionsJoined}|{Correct}";
                        File.AppendAllText(QuestionFile, NewLine + Environment.NewLine);
                        Console.WriteLine("Question added.");
                    }
                }
                else if (MenuChoice == "3")
                {
                    if (QuestionLines.Length == 0)
                    {
                        Console.WriteLine("No questions found.");
                    }
                    else
                    {
                        for (int I = 0; I < QuestionLines.Length; I++)
                        {
                            string[] Parts = QuestionLines[I].Split('|');
                            Console.WriteLine($"{I + 1}. {Parts[0]}");
                        }
                        Console.Write("Enter question number to edit: ");
                        if (int.TryParse(Console.ReadLine(), out int Num) && Num >= 1 && Num <= QuestionLines.Length)
                        {
                            Console.Write("New question text: ");
                            string Text = Console.ReadLine();
                            Console.Write("How many options? ");
                            if (int.TryParse(Console.ReadLine(), out int OptionCount))
                            {
                                List<string> Options = new List<string>();
                                for (int I = 0; I < OptionCount; I++)
                                {
                                    Console.Write($"Option {I + 1}: ");
                                    Options.Add(Console.ReadLine());
                                }
                                Console.Write("Correct answer numbers (comma separated): ");
                                string Correct = Console.ReadLine();

                                string OptionsJoined = string.Join(";", Options);
                                QuestionLines[Num - 1] = $"{Text}|{OptionsJoined}|{Correct}";
                                File.WriteAllLines(QuestionFile, QuestionLines);
                                Console.WriteLine("Question updated.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid question number.");
                        }
                    }
                }
                else if (MenuChoice == "4")
                {
                    if (QuestionLines.Length == 0)
                    {
                        Console.WriteLine("No questions found.");
                    }
                    else
                    {
                        for (int I = 0; I < QuestionLines.Length; I++)
                        {
                            string[] Parts = QuestionLines[I].Split('|');
                            Console.WriteLine($"{I + 1}. {Parts[0]}");
                        }
                        Console.Write("Enter question number to delete: ");
                        if (int.TryParse(Console.ReadLine(), out int Num) && Num >= 1 && Num <= QuestionLines.Length)
                        {
                            List<string> NewLines = new List<string>();
                            for (int I = 0; I < QuestionLines.Length; I++)
                            {
                                if (I != Num - 1)
                                {
                                    NewLines.Add(QuestionLines[I]);
                                }
                            }
                            File.WriteAllLines(QuestionFile, NewLines);
                            Console.WriteLine("Question deleted.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid question number.");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
        }
    }
    else if (MenuChoice == "5")
    {
        Console.Write("New category name: ");
        string NewCategory = Console.ReadLine();
        string QuestionFile = $"{DataFolder}questions_{NewCategory}.txt";
        if (File.Exists(QuestionFile))
        {
            Console.WriteLine("Category already exists.");
        }
        else
        {
            File.Create(QuestionFile).Close();
            Console.WriteLine("Category created.");
        }
    }
    else if (MenuChoice == "6")
    {
        Exit = true;
        Console.WriteLine("Goodbye!");
    }
    else
    {
        Console.WriteLine("Invalid menu option.");
    }
}