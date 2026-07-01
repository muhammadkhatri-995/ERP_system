using System;
namespace ERP_sys.Models
{
    public class User
    {

        public int Id { get; set; } = 1;

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public String departmentName { get; set; } = String.Empty;
        public string  roleName { get; set; } = string.Empty;

        public string designationName { get; set; } = string.Empty;

        public bool IsActive {  get; set; } 


        // forgein keys 

        // Foreign Key IDs (For Inserts / Updates)
        public int RoleId { get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }









    }
}
