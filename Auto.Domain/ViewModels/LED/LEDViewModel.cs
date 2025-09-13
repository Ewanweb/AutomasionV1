using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto.Domain.ViewModels.LED
{
    public class LEDViewModel
    {
        [Required]
        public required LedCommand  Command { get; set; }
    }

    public enum LedCommand
    {
        on,
        off,
    }
}
