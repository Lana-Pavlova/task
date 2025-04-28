using System;
using System.Collections.Generic;

namespace task;

public partial class Product
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int? ProductTypeId { get; set; }

    public string ArticleNumber { get; set; } = null!;

    public string? Description { get; set; }

    public string? Image { get; set; }

    public int? ProductionPersonCount { get; set; }

    public int? ProductionWorkshopNumber { get; set; }

    public decimal MinCostForAgent { get; set; }

    public virtual Producttype? ProductType { get; set; }

    public virtual ICollection<Productcosthistory> Productcosthistories { get; set; } = new List<Productcosthistory>();

    public virtual ICollection<Productmaterial> Productmaterials { get; set; } = new List<Productmaterial>();

    public virtual ICollection<Productsale> Productsales { get; set; } = new List<Productsale>();
}
