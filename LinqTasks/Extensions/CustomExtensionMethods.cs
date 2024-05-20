using System.Collections;
using LinqTasks.Models;

namespace LinqTasks.Extensions;

public static class CustomExtensionMethods
{
    //Put your extension methods here
    public static IEnumerable<Emp> GetEmpsWithSubordinates(this IEnumerable<Emp> emps)
    {
        IEnumerable<Emp> result = from emp in emps
            join mgr in emps on emp equals mgr.Mgr
            orderby emp.Ename, emp.Salary descending
            select emp;
        return result;
    }
}