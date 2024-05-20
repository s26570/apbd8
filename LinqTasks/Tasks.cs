using System.Collections;
using System.ComponentModel.Design;
using System.Threading.Tasks.Dataflow;
using LinqTasks.Extensions;
using LinqTasks.Models;

namespace LinqTasks;

public static partial class Tasks
{
    public static IEnumerable<Emp> Emps { get; set; }
    public static IEnumerable<Dept> Depts { get; set; }

    static Tasks()
    {
        Depts = LoadDepts();
        Emps = LoadEmps();
    }

    /// <summary>
    ///     SELECT * FROM Emps WHERE Job = "Backend programmer";
    /// </summary>
    public static IEnumerable<Emp> Task1()
    {
        IEnumerable<Emp> result = from emp in Emps where emp.Job == "Backend programmer" select emp;
        return result;
    }

    /// <summary>
    ///     SELECT * FROM Emps Job = "Frontend programmer" AND Salary>1000 ORDER BY Ename DESC;
    /// </summary>
    public static IEnumerable<Emp> Task2()
    {
        IEnumerable<Emp> result = from emp in Emps
            where emp.Job == "Frontend programmer" && emp.Salary > 1000
            orderby emp.Ename descending select emp;
        return result;
    }


    /// <summary>
    ///     SELECT MAX(Salary) FROM Emps;
    /// </summary>
    public static int Task3()
    {
        int result = (from emp in Emps select emp.Salary).Max();
        return result;
    }

    /// <summary>
    ///     SELECT * FROM Emps WHERE Salary=(SELECT MAX(Salary) FROM Emps);
    /// </summary>
    public static IEnumerable<Emp> Task4()
    {
        int maxSalary = Task3();
        IEnumerable<Emp> result = from emp in Emps where emp.Salary == maxSalary select emp;
        return result;
    }

    /// <summary>
    ///    SELECT ename AS Nazwisko, job AS Praca FROM Emps;
    /// </summary>
    public static IEnumerable<object> Task5()
    {
        IEnumerable<object> result = from emp in Emps select new { Nazwisko = emp.Ename, Praca = emp.Job };
        return result;
    }

    /// <summary>
    ///     SELECT Emps.Ename, Emps.Job, Depts.Dname FROM Emps
    ///     INNER JOIN Depts ON Emps.Deptno=Depts.Deptno
    ///     Rezultat: Złączenie kolekcji Emps i Depts.
    /// </summary>
    public static IEnumerable<object> Task6()
    {
        IEnumerable<object> result = from emp in Emps
            join dept in Depts on emp.Deptno equals dept.Deptno
            select new { emp.Ename, emp.Job, dept.Dname };
        return result;
    }

    /// <summary>
    ///     SELECT Job AS Praca, COUNT(1) LiczbaPracownikow FROM Emps GROUP BY Job;
    /// </summary>
    public static IEnumerable<object> Task7()
    {
        IEnumerable<object> result = from emp in Emps
            group emp by emp.Job
            into gr
            select new { Praca = gr.Key, LiczbaPracownikow = gr.Count() };
        return result;
    }

    /// <summary>
    ///     Zwróć wartość "true" jeśli choć jeden
    ///     z elementów kolekcji pracuje jako "Backend programmer".
    /// </summary>
    public static bool Task8()
    {
        int number = (from emp in Emps where emp.Job == "Backend programmer" select emp).Count();
        if (number > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    ///     SELECT TOP 1 * FROM Emp WHERE Job="Frontend programmer"
    ///     ORDER BY HireDate DESC;
    /// </summary>
    public static Emp Task9()
    {
        Emp result =
            (from emp in Emps where emp.Job == "Frontend programmer" orderby emp.HireDate descending select emp)
            .First();
        return result;
    }

    /// <summary>
    ///     SELECT Ename, Job, Hiredate FROM Emps
    ///     UNION
    ///     SELECT "Brak wartości", null, null;
    /// </summary>
    public static IEnumerable<object> Task10()
    {
        IEnumerable<object> result = (from emp in Emps select new { emp.Ename, emp.Job, emp.HireDate })
            .Union(new[] { new { Ename = "Brak wartości", Job = (string)null, HireDate = (DateTime?)null } })
            .ToList();
        return result;
    }

    /// <summary>
    ///     Wykorzystując LINQ pobierz pracowników podzielony na departamenty pamiętając, że:
    ///     1. Interesują nas tylko departamenty z liczbą pracowników powyżej 1
    ///     2. Chcemy zwrócić listę obiektów o następującej srukturze:
    ///     [
    ///     {name: "RESEARCH", numOfEmployees: 3},
    ///     {name: "SALES", numOfEmployees: 5},
    ///     ...
    ///     ]
    ///     3. Wykorzystaj typy anonimowe
    /// </summary>
    public static IEnumerable<object> Task11()
    {
        IEnumerable<object> result = (from emp in Emps
            join dept in Depts on emp.Deptno equals dept.Deptno
            group dept by dept.Dname
            into deptGroup
            where deptGroup.Count() > 1
            select new { name = deptGroup.Key, numOfEmployees = deptGroup.Count() }).ToList();
        return result;
    }

    /// <summary>
    ///     Napisz własną metodę rozszerzeń, która pozwoli skompilować się poniższemu fragmentowi kodu.
    ///     Metodę dodaj do klasy CustomExtensionMethods, która zdefiniowana jest poniżej.
    ///     Metoda powinna zwrócić tylko tych pracowników, którzy mają min. 1 bezpośredniego podwładnego.
    ///     Pracownicy powinny w ramach kolekcji być posortowani po nazwisku (rosnąco) i pensji (malejąco).
    /// </summary>
    public static IEnumerable<Emp> Task12()
    {
        IEnumerable<Emp> result = Emps.GetEmpsWithSubordinates();
        
        return result;
    }

    /// <summary>
    ///     Poniższa metoda powinna zwracać pojedyczną liczbę int.
    ///     Na wejściu przyjmujemy listę liczb całkowitych.
    ///     Spróbuj z pomocą LINQ'a odnaleźć tę liczbę, które występuja w tablicy int'ów nieparzystą liczbę razy.
    ///     Zakładamy, że zawsze będzie jedna taka liczba.
    ///     Np: {1,1,1,1,1,1,10,1,1,1,1} => 10
    /// </summary>
    public static int Task13(int[] arr)
    {
        int result =
            (from number in arr
                group number by number
                into numberGroup
                where numberGroup.Count() % 2 != 0
                select numberGroup.Key).FirstOrDefault();
        return result;
    }

    /// <summary>
    ///     Zwróć tylko te departamenty, które mają 5 pracowników lub nie mają pracowników w ogóle.
    ///     Posortuj rezultat po nazwie departament rosnąco.
    /// </summary>
    public static IEnumerable<Dept> Task14()
    {
        IEnumerable<Dept> result = from dept in Depts
            join emp in Emps on dept.Deptno equals emp.Deptno into empGroup
            where empGroup.Count() == 0 || empGroup.Count() == 5
            orderby dept.Dname
            select dept;    
        return result;
    }
}