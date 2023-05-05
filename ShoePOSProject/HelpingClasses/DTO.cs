using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.HelpingClasses
{
    public class DTO
    {
        public int Id { get; set; }
        public string EncryptedId { get; set; }
        public string Brand { get; set; }
        public string Size { get; set; }
        public string ShoeStyle { get; set; }
        public string Collection { get; set; }
        public string Color { get; set; }
        public string Gender { get; set; }
        public string Sides { get; set; }
        public string LoadingInstruction { get; set; }
        public string Fence { get; set; }
        public string Access { get; set; }
        public string Address { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Price { get; set; }
        public string CreatedBy { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string InventoryDate { get; set; }
        public string DaysofLot { get; set; }
        public int? isActive { get; set; }
        public int? Role { get; set; }
        public string SalePrice { get; set; }
        public double TotalPrice { get; set; }
        public double TotalSalePrice { get; set; }

        //Customer DTO List

        public int CustomerId { get; set; }
        public string CustomerEncryptedId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPrimaryPhone { get; set; }
        public string CustomerMailingAddress { get; set; }
        public string CustomerSecondaryPhone { get; set; }
        public string CustomerMailingCity { get; set; }
        public string CustomerMailingState { get; set; }
        public string CustomerMailingZip { get; set; }
        public string CustomerMailingCounty { get; set; }
        public string CustomerPhysicalAddress { get; set; }
        public string CustomerPhysicalState { get; set; }
        public string CustomerPhysicalZip { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerDateofBirth { get; set; }
        public string CustomerSocialSecurity { get; set; }
        public string CustomerReferenceName { get; set; }
        public string CustomerReferencePhone { get; set; }
        public string CustomerExtra1 { get; set; }
        public string CustomerExtra2 { get; set; }
        public string CustomerExtra3 { get; set; }
        public string CustomerCreatedBy { get; set; }

        //Customer Sales DTO Lost
        public int CustomerSalesId { get; set; }
        public string CustomerSalesEncryptedId { get; set; }
        public string CustomerSalesCashPrice { get; set; }
        public string CustomerSalesSerialNumber { get; set; }
        public string CustomerSalesCondition { get; set; }
        public string CustomerUsedDamage { get; set; }
        public string CustomerSalesOwnership { get; set; }
        public string CustomerSalesCreatedBy { get; set; }

        //BSST DTO

        public int BSSTId { get; set; }
        public string BSSTEncryptedId { get; set; }
        public string BSSTName { get; set; }
        public string BSSTBrand { get; set; }
        public string BSSTStyle { get; set; }
        public string BSSTSize { get; set; }
        public string BSSTType { get; set; }
        public string BSSTCategory { get; set; }
        public string BSSTEncryptedCategoryId { get; set; }
        public int? BSSTCategoryId { get; set; }

        //New Option DTO

        public int OptionId { get; set; }
        public string OptionEncryptedId { get; set; }
        public string OptionName { get; set; }
        public string OptionPrice { get; set; }
    }
}