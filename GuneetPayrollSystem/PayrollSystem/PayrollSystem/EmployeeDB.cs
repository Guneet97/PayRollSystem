///////////////////////////////////////////////////////
// TINFO 200 , Winter 2017
// UW Tacoma Institute of Technology, Guneet Bawa
// 2017-02-26 - Programming Project 3 - Employee DB
// This program creates an employee database
//////////////////////////////////////////////////////
// Change History
// -- Date --- Developer -- Description
// 02-21-17 - bawag-  Wrote and transfered features from the estudent DB to employee DB
// 02-23-17 - bawag - Worked on the create employee method 
// 02-24-17 - bawag - Finished create employee and started the delete and update method
// 02-26-17 - bawag  - Finished the delete method
using System;
using System.IO;

internal class EmployeeDB
{
    public const char CREATE_C = 'C';
    public const char CREATE_c = 'c';
    public const char EMP_H = 'H';
    public const char EMP_h = 'h';
    public const char EMP_C = 'C';
    public const char EMP_c = 'c';
    public const char EMP_P = 'P';
    public const char EMP_p = 'p';
    public const char EMP_S = 'S';
    public const char EMP_s = 's';
    public const char Delete_D = 'D';
    public const char Delete_d = 'd';
    public const string SAL = "SAL";
    public const string BPC = "BPC";
    public const string COM = "COM";
    public const string HOURLY = "HOURLY";

    private Employee[] employees;
    public EmployeeDB()
    {
    }
    internal void ReadDataFromInputFile()
    {
        // create and intialize the file objects
        FileStream fstream = new FileStream("INPUT.txt", FileMode.Open, FileAccess.Read);
        StreamReader infile = new StreamReader(fstream);   // FileStream

        int numberOfRecords = int.Parse(infile.ReadLine());
        employees = new Employee[numberOfRecords];

        for (int i = 0; i < employees.Length; i++)
        {
            string employeeType = infile.ReadLine();

            // read in data for an employee
            string fName = infile.ReadLine();
            string lName = infile.ReadLine();
            string ssn = infile.ReadLine();

            // how many more things are there to read?
            if (employeeType == SAL)
            {
                decimal weeklySalary = decimal.Parse(infile.ReadLine());

                // make a employee using the data you just read
                // put the employee into the array
                employees[i] = new SalariedEmployee(fName, lName, ssn, weeklySalary);

            }
            else if (employeeType == BPC)
            {
                decimal grossSales = decimal.Parse(infile.ReadLine());
                decimal commissionRate = decimal.Parse(infile.ReadLine());
                decimal baseSalary = decimal.Parse(infile.ReadLine());
                // make an employee using the data you just read
                // put the employee into the array
                employees[i] = new BasePlusCommissionEmployee(fName, lName, ssn, grossSales, commissionRate, baseSalary);
            }
            else if (employeeType == COM)
            {
                decimal grossSales = decimal.Parse(infile.ReadLine());
                decimal commissionRate = decimal.Parse(infile.ReadLine());

                // make a employee using the data you just read
                // put the employee into the array
                employees[i] = new CommissionEmployee(fName, lName, ssn, grossSales, commissionRate);
            }
            else if (employeeType == HOURLY)
            {
                decimal hourlyWage = decimal.Parse(infile.ReadLine());
                decimal hoursWorked = decimal.Parse(infile.ReadLine());

                // make a employee using the data you just read
                // put the employee into the array
                employees[i] = new HourlyEmployee(fName, lName, ssn, hourlyWage, hoursWorked);
            }
            else
            {
                Console.WriteLine("ERROR: That is not a valid employee type.");
            }
        }

        // close the file or release the resource
        infile.Close();
    }
    public void PrintAllRecords()
    {
        Console.WriteLine("** Contents of db ***************");
        foreach (var emp in employees)
        {
            Console.WriteLine(emp);
        }
    }//END method
    internal void WriteDataToOutputFile()
    {
        // create and open an output file
        FileStream fstream = new FileStream("OUTPUT.txt", FileMode.Create, FileAccess.Write);
        StreamWriter outfile = new StreamWriter(fstream);

        // write the size of the array
        outfile.WriteLine(employees.Length);

        // write the data from the objects in the array to the output file
        foreach (var employee in employees)
        {
            if (employee is HourlyEmployee)
            {
                var hourly = (HourlyEmployee)employee;
                outfile.Write(hourly.ToDataFileString());
            }
            else if (employee is SalariedEmployee)
            {
                var sal = (SalariedEmployee)employee;
                outfile.Write(sal.ToDataFileString());
            }
            else if (employee is CommissionEmployee)
            {
                var com = (CommissionEmployee)employee;
                outfile.Write(com.ToDataFileString());
            }
            else if (employee is BasePlusCommissionEmployee)
            {
                var bpc = (BasePlusCommissionEmployee)employee;
                outfile.Write(bpc.ToDataFileString());
            }
        }

        // close the output file
        outfile.Close();
    }
    // main method that operates the application once we get
    // all the data read into it
    internal void OperateDatabase()
    {
        // explain the program to the user 
        DisplayProgramExplanation();

        ConsoleKeyInfo choice;
        do
        {
            // user interface
            PresentUserInterface();
            //string choice = Console.ReadLine();
            //selection = choice[0];

            choice = Console.ReadKey();

            switch (choice.KeyChar)
            {
                case CREATE_C:
                case CREATE_c:
                    CreateEmployeeRecord();
                    break;
                case 'R':
                case 'r':
                    FindAndPrintEmployeeRecord();
                    break;
                case 'U':
                case 'u':
                    UpdateEmployeeRecord();
                    break;
                case 'D':
                case 'd':
                    DeleteEmployeeRecord();
                    break;
                case 'P':
                case 'p':
                    PrintAllEmployeeRecords();
                    break;
                default:
                    break;
            }

        } while ((choice.KeyChar != 'Q') && (choice.KeyChar != 'q'));

    }
    //display user interface
    private void PresentUserInterface()
    {
        Console.WriteLine(@"
Select from the following options:
[C]reate (a new employee)
[R]ead (read a record)
[U]pdate
[D]elete
[P]rint all records in the database
[Q]uit
");
    }
    //display prompt intro
    private void DisplayProgramExplanation()
    {
        Console.WriteLine(@"
********************************************
Welcome to the Employee Database program.
You can execute some of typical database features,
including
Create, Read Update, and Delete
for the employee records that are present.
");
    }

    private void CreateEmployeeRecord()
    {

        //display prompt that asks for employee last name
        Console.Write("Enter the Last Name for the record to add: (ex: Smith) ");

        //user types in the employee last name
        string lname = Console.ReadLine();

        // check to see if record already exists in the database
        // if it does, return back to main menu, issue a message
        Employee emp = FindEmployeeRecord(lname);
        if (emp != null)
        {
            Console.WriteLine("Error: " + lname + " is already in the database.");
            return;
        }

        //display prompt that asks for type of employee
        Console.WriteLine("Enter the type of employee H= Hourly, C= Commision, S = Salaried, P= BasePlusCommision: ");
        ConsoleKeyInfo employeeType = Console.ReadKey();

        //display prompt that asks for employee First Name
        Console.WriteLine("Enter the employees first name: ");
        string fname = Console.ReadLine();
        //display prompt that asks for employee Last Name
        Console.WriteLine("Enter the employees last name: ");
        lname = Console.ReadLine();

        //display prompt that asks for the SSN
        Console.WriteLine("Enter the employees SSN: ");
        string ssn = Console.ReadLine();

        // if type of employee is H
        if (employeeType.KeyChar == EMP_H || employeeType.KeyChar == EMP_h)
        {
            //display prompts for hourly wage and hours worked
            Console.WriteLine("Enter hourly wage: ");
            decimal hourlyWage = decimal.Parse(Console.ReadLine());
            Console.WriteLine("Enter hours worked: ");
            decimal hoursWorked = decimal.Parse(Console.ReadLine());

            // make hourly employee
            emp = new HourlyEmployee(fname, lname, ssn, hourlyWage, hoursWorked);
        }
        //if type employee is salaried
        else if (employeeType.KeyChar == EMP_S || employeeType.KeyChar == EMP_s)
        {
            // prompt for weekly salary
            Console.WriteLine("Enter the weekly salary: ");
            decimal weeklySalary = decimal.Parse(Console.ReadLine());

            // make a salaried employee
            emp = new SalariedEmployee(fname, lname, ssn, weeklySalary);
        }
        //if employee type is commision
        else if (employeeType.KeyChar == EMP_C || employeeType.KeyChar == EMP_c)
        {
            // prompt for gross sales and commision rate
            Console.WriteLine("Enter the gross sales: ");
            decimal grossSales = decimal.Parse(Console.ReadLine());
            Console.WriteLine("Enter the commision rate: ");
            decimal commisionRate = decimal.Parse(Console.ReadLine());
            // make a commision employee
            emp = new CommissionEmployee(fname, lname, ssn, grossSales, commisionRate);
        }
        //if employee type is BasePlusCommision
        else if (employeeType.KeyChar == EMP_P || employeeType.KeyChar == EMP_p)
        {
            // prompt for gross sales, commision rate, and base salary
            Console.WriteLine("Enter the gross sales: ");
            decimal grossSales = decimal.Parse(Console.ReadLine());
            Console.WriteLine("Enter the commision rate: ");
            decimal commisionRate = decimal.Parse(Console.ReadLine());
            Console.WriteLine("Enter Base salary: ");
            decimal baseSalary = decimal.Parse(Console.ReadLine());
            // make a BasePluseCommision employee
            emp = new BasePlusCommissionEmployee(fname, lname, ssn, grossSales, commisionRate, baseSalary);
        }
        //if employee types does not exist
        else
        {
            Console.WriteLine(employeeType.KeyChar + " is not a type of employee.");
            return;
        }
        ////display the current employee data and ask for confirm
        //// ask user to confirm
        Console.WriteLine(emp);
        Console.WriteLine("Is the above information all correct? [Y] or [N] ");
        string answer = Console.ReadLine();
        if (answer != "Y")
        {
            return;
        }


        // the db saves the record, and returns to main menu - steps:
        // and insert the newly created student object into the array

        // 1 - make an array that is 1 "bigger" than employees
        Employee[] biggerEmployeeArray = new Employee[employees.Length + 1];

        // 2 - copy all objects from employees to the bigger array
        // (at the same index val)
        for (int i = 0; i < employees.Length; i++)
        {
            biggerEmployeeArray[i] = employees[i];
        }

        // put emp in the last slot in the bigger array
        biggerEmployeeArray[biggerEmployeeArray.Length - 1] = emp;

        // make the employees ref point to the bigger array
        employees = biggerEmployeeArray;

    }
    //did not get this feature to work without compilation errors
    private void UpdateEmployeeRecord()
    {
        Console.WriteLine("Feature Not Available");
    }

    private void DeleteEmployeeRecord()
    {
        Console.Write("Enter the last name to delete:(ex: Smith) ");

        //user types in the employee last name
        string lname = Console.ReadLine();
        Employee emp = FindEmployeeRecord(lname);
        if (emp == null)
        {
            Console.WriteLine("Error: " + lname + " is not in the database.");
            return;
        }
        //print the employee and ask for conformation
        Console.WriteLine(emp);
        Console.WriteLine(" /n Is this the employee you want to delete? [Y] or [N] ");
        string answer = Console.ReadLine();
        if (answer != "Y")
        {
            return;
        }

        //make a smaller employee array
        //making the part that deletes the employee
        Employee[] smallerEmployeeArray = new Employee[employees.Length - 1];
        //setting a true and false bool for the if statements
        bool normalcase = true;
        for (int i = 0; i < employees.Length; i++)
        {

            if (emp != employees[i])
            {
                //ref smaller array to employee array
                if (normalcase)
                {

                    smallerEmployeeArray[i] = employees[i];
                }
                //delete employee from the smaller array and then ref that to employee array
                else
                {
                    smallerEmployeeArray[i - 1] = employees[i];
                }
            }
            else
            {
                normalcase = false;
            }
        }
        // make the employees ref point to the new smaller array
        employees = smallerEmployeeArray;

    }

    //did not work on this feature
    private void PrintAllEmployeeRecords()
    {
        Console.WriteLine("Feature Not Available");
    }


    private void FindAndPrintEmployeeRecord()
    {
        //searching for employee
        Console.WriteLine("Enter the employee last name: ");
        string name = Console.ReadLine();
        Employee emp = FindEmployeeRecord();

        //process when employee is not found and found
        if (emp == null)
        {
            Console.WriteLine("Employee name" + name + "was not found");
        }
        else
        {
            Console.WriteLine(emp);
        }
    }

    //empty constructor to be used in find and print employee
    private Employee FindEmployeeRecord()
    {
        throw new NotImplementedException();
    }

    private Employee FindEmployeeRecord(string Lastname)
    {
        // look through the collection at each employee
        foreach (var emp in employees)
        {
            // if we find an employee with a matching last name
            if (emp.LastName == Lastname)
            {
                // return the object
                return emp;
            }
        }

        // if we get through the entire collection with no student
        // object find that matches the search last name, 
        // so return a null reference
        return null;
    }
}








