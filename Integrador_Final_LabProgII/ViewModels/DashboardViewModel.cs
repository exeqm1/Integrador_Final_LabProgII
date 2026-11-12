namespace ITManager.ViewModels
{
    public class DashboardViewModel
    {
        // KPIs Generales
        public int TotalTicketsAbiertos { get; set; }
        public int TotalTicketsEnProceso { get; set; }
        public int TotalTicketsCriticos { get; set; }
        public int TotalEquiposOperativos { get; set; }
        public int TotalEquiposEnReparacion { get; set; }
        public int TotalEquiposDeBaja { get; set; }
        public int TotalRepuestosCriticos { get; set; }
        public int TotalRepuestosAgotados { get; set; }

        // Datos para Gráfico 1: Tickets por Estado
        public int TicketsAbiertos { get; set; }
        public int TicketsEnProceso { get; set; }
        public int TicketsResueltos { get; set; }
        public int TicketsCerrados { get; set; }

        // Datos para Gráfico 2: Mantenimientos por Tipo o Categoría
        public List<string> CategoriasLabels { get; set; } = new();
        public List<int> MantenimientosPorCategoria { get; set; } = new();
    }
}

