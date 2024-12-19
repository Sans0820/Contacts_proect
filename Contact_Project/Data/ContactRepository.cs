using Contact_Project.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Contact_Project.Data
{
    public class ContactRepository
    {
        private readonly string _connectionString;

        public ContactRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ContactDBConnection"].ConnectionString;
        }

        // Retrieve all contacts
        public List<Contact> GetAllContacts()
        {
            var contacts = new List<Contact>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Contacts_Table";
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contacts.Add(new Contact
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Prefix = reader["Prefix"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                ContactType = reader["ContactType"].ToString(),
                                Organization = reader["Organization"].ToString(),
                                JobTitle = reader["JobTitle"].ToString(),
                                Department = reader["Department"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString(),  // New field
                                Address = reader["Address"].ToString(),  // New field
                                ImagePath = reader["ImagePath"].ToString(),
                            });
                        }
                    }
                }
            }
            return contacts;
        }
   
        public Contact GetContactById(int id)
        {
            Contact contact = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Contacts_Table WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            contact = new Contact
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Prefix = reader["Prefix"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                ContactType = reader["ContactType"].ToString(),
                                Organization = reader["Organization"].ToString(),
                                JobTitle = reader["JobTitle"].ToString(),
                                Department = reader["Department"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString(),  // New field
                                Address = reader["Address"].ToString(),  // New field
                                ImagePath = reader["ImagePath"].ToString(),
                                Website= reader["Website"].ToString()
                            };
                        }
                    }
                }
            }
            return contact;
        }

        // Add a new contact
        public void AddContact(Contact contact)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"INSERT INTO Contacts_Table 
                    (Prefix, FirstName, LastName, ContactType, Organization, JobTitle, Department, Email, Phone, Address, ImagePath,Website) 
                    VALUES (@Prefix, @FirstName, @LastName, @ContactType, @Organization, @JobTitle, @Department, @Email, @Phone, @Address, @ImagePath,@Website)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", contact.FirstName);
                    command.Parameters.AddWithValue("@LastName", contact.LastName);
                    command.Parameters.AddWithValue("@Email", contact.Email);
                    command.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(contact.Phone) ? (object)DBNull.Value : contact.Phone);  // New field
                    command.Parameters.AddWithValue("@Address", string.IsNullOrEmpty(contact.Address) ? (object)DBNull.Value : contact.Address);  // New field
                    command.Parameters.AddWithValue("@JobTitle", string.IsNullOrEmpty(contact.JobTitle) ? (object)DBNull.Value : contact.JobTitle);
                    command.Parameters.AddWithValue("@Organization", string.IsNullOrEmpty(contact.Organization) ? (object)DBNull.Value : contact.Organization);
                    command.Parameters.AddWithValue("@ContactType", string.IsNullOrEmpty(contact.ContactType) ? (object)DBNull.Value : contact.ContactType);
                    command.Parameters.AddWithValue("@Department", string.IsNullOrEmpty(contact.Department) ? (object)DBNull.Value : contact.Department);
                    command.Parameters.AddWithValue("@Prefix", string.IsNullOrEmpty(contact.Prefix) ? (object)DBNull.Value : contact.Prefix);
                    command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(contact.ImagePath) ? (object)DBNull.Value : contact.ImagePath);
                    command.Parameters.AddWithValue("@Website", string.IsNullOrEmpty(contact.Website) ? (object)DBNull.Value : contact.Website);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        // Update an existing contact
        public void UpdateContact(Contact contact)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"UPDATE Contacts_Table 
                     SET Prefix = @Prefix, FirstName = @FirstName, LastName = @LastName, ContactType = @ContactType, 
                         Organization = @Organization, JobTitle = @JobTitle, Department = @Department, 
                         Email = @Email, Phone = @Phone, Address = @Address, ImagePath = @ImagePath, Website=@Website
                     WHERE Id = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", contact.Id);
                    command.Parameters.AddWithValue("@FirstName", contact.FirstName);
                    command.Parameters.AddWithValue("@LastName", contact.LastName);
                    command.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(contact.Phone) ? (object)DBNull.Value : contact.Phone);  // New field
                    command.Parameters.AddWithValue("@Address", string.IsNullOrEmpty(contact.Address) ? (object)DBNull.Value : contact.Address);  // New field
                    command.Parameters.AddWithValue("@Organization", string.IsNullOrEmpty(contact.Organization) ? (object)DBNull.Value : contact.Organization);
                    command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(contact.Email) ? (object)DBNull.Value : contact.Email);
                    command.Parameters.AddWithValue("@ContactType", string.IsNullOrEmpty(contact.ContactType) ? (object)DBNull.Value : contact.ContactType);
                    command.Parameters.AddWithValue("@Department", string.IsNullOrEmpty(contact.Department) ? (object)DBNull.Value : contact.Department);
                    command.Parameters.AddWithValue("@JobTitle", string.IsNullOrEmpty(contact.JobTitle) ? (object)DBNull.Value : contact.JobTitle);
                    command.Parameters.AddWithValue("@Prefix", string.IsNullOrEmpty(contact.Prefix) ? (object)DBNull.Value : contact.Prefix);
                    command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(contact.ImagePath) ? (object)DBNull.Value : contact.ImagePath);
                    command.Parameters.AddWithValue("@Website", string.IsNullOrEmpty(contact.Website) ? (object)DBNull.Value : contact.Website);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception("Update failed. Contact not found.");
                    }
                }
            }
        }

        // Delete a contact
        public void DeleteContact(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "DELETE FROM Contacts_Table WHERE Id = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        // Retrieve all contact types
        public List<ContactType> GetAllContactTypes()
        {
            var contactTypes = new List<ContactType>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM ContactTypes"; // Ensure the table name matches your database
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contactTypes.Add(new ContactType
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString()
                            });
                        }
                    }
                }
            }
            return contactTypes;
        }

    }
}
