using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MiniProject4.Domain.Models;

namespace MiniProject4.Persistence.Models;

[PrimaryKey("Empno", "Projno")]
[Table("workson")]
[Index("Projno", Name = "IX_workson_projno")]
public partial class Workson
{
    [Key]
    [Column("empno")]
    public int Empno { get; set; }

    [Key]
    [Column("projno")]
    public int? Projno { get; set; }

    [Column("dateworked")]
    public DateOnly? Dateworked { get; set; }

    [Column("hoursworked")]
    public int? Hoursworked { get; set; }

    [ForeignKey("Empno")]
    [InverseProperty("Worksons")]
    public virtual Employee? EmpnoNavigation { get; set; }

    [ForeignKey("Projno")]
    [InverseProperty("Worksons")]
    public virtual Project? ProjnoNavigation { get; set; }

    [NotMapped]
    public DateDto? DateWorkedObject { get; set; }

    public void ConvertDateWorkedObjectToDateOnly()
    {
        if (DateWorkedObject != null)
        {
            Dateworked = new DateOnly(DateWorkedObject.Year, DateWorkedObject.Month, DateWorkedObject.Day);
        }
    }
}
