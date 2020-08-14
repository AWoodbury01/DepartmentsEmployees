using DepartmentsEmployees.Models;
using System.Collections.Generic;
using System.Data.SqlClient;


namespace DepartmentsEmployees.Data
{
    class EmployeeRepository
    {
        private List<Department> employee;

        public SqlConnection Connection { get; private set; }

        public List<Department> GetAllEmployees()
        {
            //  We must "use" the database connection.
            //  Because a database is a shared resource (other applications may be using it too) we must
            //  be careful about how we interact with it. Specifically, we Open() connections when we need to
            //  interact with the database and we Close() them when we're finished.
            //  In C#, a "using" block ensures we correctly disconnect from a resource even if there is an error.
            //  For database connections, this means the connection will be properly closed.
            using (SqlConnection conn = Connection)
            {
                // Note, we must Open() the connection, the "using" block   doesn't do that for us.
                conn.Open();

                // We must "use" commands too.
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Here we setup the command with the SQL we want to execute before we execute it.
                    cmd.CommandText = "SELECT Id, FirstName, LastName FROM Employee";

                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    SqlDataReader reader = cmd.ExecuteReader();

                    // A list to hold the departments we retrieve from the database.
                    List<Employee> employees = new List<Employee>();

                    // Read() will return true if there's more data to read
                    while (reader.Read())
                    {
                        // The "ordinal" is the numeric position of the column in the query results.
                        //  For our query, "Id" has an ordinal value of 0 and "DeptName" is 1.
                        int idColumnPosition = reader.GetOrdinal("Id");

                        // We user the reader's GetXXX methods to get the value for a particular ordinal.
                        int idValue = reader.GetInt32(idColumnPosition);

                        int FirstNameColumnPosition = reader.GetOrdinal("FirstName");
                        string FirstNameValue = reader.GetString(FirstNameColumnPosition);
                        int LastNameColumnPosition = reader.GetOrdinal("LastName");
                        string LastNameValue = reader.GetString(LastNameColumnPosition);


                        // Now let's create a new department object using the data from the database.
                        Employee employee = new Employee
                        {
                            Id = idValue,
                            FirstName = FirstNameValue,
                            LastName = LastNameValue
                        };

                        // ...and add that department object to our list.
                        employees.Add(employee);
                    }

                    // We should Close() the reader. Unfortunately, a "using" block won't work here.
                    reader.Close();

                    // Return the list of departments who whomever called this method.
                    return employee;
                }
            }

        }

        /// <summary>
        ///  Returns a single department with the given id.
        /// </summary>
        public Employee GetEmployeeById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT FirstName, LastName FROM Employee WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = null;

                    // If we only expect a single row back from the database, we don't need a while loop.
                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            Id = id,
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString
                 (reader.GetOrdinal("LastName"))
                        };
                    }

                    reader.Close();

                    return employee;
                }
            }
        }

        /// <summary>
        ///  Add a new department to the database
        ///   NOTE: This method sends data to the database,
        ///   it does not get anything from the database, so there is nothing to return.
        /// </summary>
        public void AddEmployee(Employee employee)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // These SQL parameters are annoying. Why can't we use string interpolation?
                    // ... sql injection attacks!!!
                    cmd.CommandText = "INSERT INTO Employee (FirstName) OUTPUT INSERTED.Id Values (@FirstName)";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", employee.FirstName));
                    cmd.CommandText = "INSERT INTO Employee (LastFirstName) OUTPUT INSERTED.Id Values (@LastName)";
                    cmd.Parameters.Add(new SqlParameter("@LastName", employee.LastName));

                    int id = (int)cmd.ExecuteScalar();

                    employee.Id = id;
                }
            }

            // when this method is finished we can look in the database and see the new department.
        }

        /// <summary>
        ///  Updates the department with the given id
        /// </summary>
        public void UpdateEmployee(int id, Employee employee)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Employee
                                     SET FirstName = @FirstName
                                     WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", employee.FirstName));
                    cmd.CommandText = @"UPDATE Employee
                                     SET LastName = @LastName
                                     WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@LastName", employee.LastName));

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        ///  Delete the department with the given id
        /// </summary>
        public void DeleteEmployee(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Employee WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}
