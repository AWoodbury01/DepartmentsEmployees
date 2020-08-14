using DepartmentsEmployees.Data;
using DepartmentsEmployees.Models;
using System;
using System.Collections.Generic;

namespace DepartmentsEmployees
{
    class Program
    {
        static void Main(string[] args)
        {
            DepartmentRepository departmentRepo = new DepartmentRepository();

            Console.WriteLine("Getting All Departments:");
            Console.WriteLine();

            List<Department> allDepartments = departmentRepo.GetAllDepartments();

            foreach (Department dept in allDepartments)
            {
                Console.WriteLine($"{dept.Id} {dept.DeptName}");
            }

            Console.WriteLine("----------------------------");
            Console.WriteLine("Getting Department with Id 1");

            Department singleDepartment = departmentRepo.GetDepartmentById(1);

            Console.WriteLine($"{singleDepartment.Id} {singleDepartment.DeptName}");




            Department legalDept = new Department
            {
                DeptName = "Legal"
            };

            departmentRepo.AddDepartment(legalDept);

            Console.WriteLine("-------------------------------");
            Console.WriteLine("Added the new Legal Department!");

            departmentRepo.UpdateDepartment(1, new Department() { DeptName = "New Department" });

            Console.WriteLine("Here's your updated department:");
            Console.WriteLine(departmentRepo.GetDepartmentById(1).DeptName);

            Console.WriteLine("Now let's delete a department.");
            departmentRepo.DeleteDepartment(4);
            List<Department> newDeps = departmentRepo.GetAllDepartments();
            foreach (Department dep in newDeps)
            {
                Console.WriteLine(dep.DeptName);
            }

            Console.ReadLine();
        }
    }
}