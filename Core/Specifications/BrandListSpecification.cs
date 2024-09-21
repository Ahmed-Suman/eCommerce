using Core.Entities;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Core.Specifications;

public class BrandListSpecification : BaseSpecification<Product,string>
{
    public BrandListSpecification()
    {
        AddSelect( x => x.Brand);
        ApplyDistinct();
    }
}
