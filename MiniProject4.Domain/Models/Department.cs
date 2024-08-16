using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MiniProject4.Persistence.Models;

[Table("department")]
[Index("Mgrempno", Name = "IX_department_mgrempno")]
public partial class Department
{
    [Key]
    [Column("deptno")]
    public int Deptno { get; set; }

    [Column("deptname")]
    [StringLength(255)]
    public string Deptname { get; set; } = null!;

    [Column("mgrempno")]
    public int Mgrempno { get; set; }

    [ForeignKey("Mgrempno")]
    [InverseProperty("Departments")]
    public virtual Employee? MgrempnoNavigation { get; set; }

    [NotMapped]
    [InverseProperty("DeptnoNavigation")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
