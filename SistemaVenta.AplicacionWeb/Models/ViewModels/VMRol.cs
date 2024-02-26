using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.ViewModels
{
    public class VMRol
    {

        public int IdRol { get; set; }

        public string? Descripcion { get; set; }

        public bool? EsActivo { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public virtual ICollection<RolMenu> RolMenus { get; set; } = new List<RolMenu>();

        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
