using OrderCloud.SDK;

namespace OC_Seeding_Project.EnvironmentSeed
{
    public class SeedConstants
    {
        public static readonly List<SecurityProfile> DefaultSecurityProfiles = new List<SecurityProfile>() {
			
			// seller/supplier
			new SecurityProfile() { ID = CustomRole.CustomBuyerAdmin, CustomRoles = new CustomRole[] { CustomRole.CustomBuyerAdmin }, Roles = new ApiRole[] { ApiRole.AddressAdmin, ApiRole.ApprovalRuleAdmin, ApiRole.BuyerAdmin, ApiRole.BuyerUserAdmin, ApiRole.CreditCardAdmin, ApiRole.UserGroupAdmin } },
            new SecurityProfile() { ID = CustomRole.BuyerImpersonator, CustomRoles = new CustomRole[] { CustomRole.BuyerImpersonator }, Roles = new ApiRole[] { ApiRole.BuyerImpersonation } },
            new SecurityProfile() { ID = CustomRole.CustomCategoryAdmin, CustomRoles = new CustomRole[] { CustomRole.CustomCategoryAdmin }, Roles = new ApiRole[] { ApiRole.CategoryAdmin } },
            new SecurityProfile() { ID = CustomRole.ContentAdmin, CustomRoles = new CustomRole[] { CustomRole.AssetAdmin, CustomRole.DocumentAdmin, CustomRole.SchemaAdmin }, Roles = new ApiRole[] { ApiRole.ApiClientAdmin } },
            new SecurityProfile() { ID = CustomRole.CustomMeAdmin, CustomRoles = new CustomRole[] { CustomRole.CustomMeAdmin }, Roles = new ApiRole[] { ApiRole.MeAdmin, ApiRole.MeXpAdmin } },
            new SecurityProfile() { ID = CustomRole.MeSupplierAddressAdmin, CustomRoles = new CustomRole[] { CustomRole.MeSupplierAddressAdmin }, Roles = new ApiRole[] { ApiRole.SupplierAddressAdmin, ApiRole.SupplierReader } },
            new SecurityProfile() { ID = CustomRole.MeSupplierAdmin, CustomRoles = new CustomRole[] { CustomRole.AssetAdmin, CustomRole.MeSupplierAdmin }, Roles = new ApiRole[] { ApiRole.SupplierAdmin, ApiRole.SupplierReader } },
            new SecurityProfile() { ID = CustomRole.MeSupplierUserAdmin, CustomRoles = new CustomRole[] { CustomRole.MeSupplierUserAdmin }, Roles = new ApiRole[] { ApiRole.SupplierReader, ApiRole.SupplierUserAdmin } },
            new SecurityProfile() { ID = CustomRole.CustomOrderAdmin, CustomRoles = new CustomRole[] { CustomRole.CustomOrderAdmin }, Roles = new ApiRole[] { ApiRole.AddressReader, ApiRole.OrderAdmin, ApiRole.ShipmentReader } },
            new SecurityProfile() { ID = CustomRole.CustomProductAdmin, CustomRoles = new CustomRole[] { CustomRole.CustomProductAdmin }, Roles = new ApiRole[] { ApiRole.AdminAddressAdmin, ApiRole.AdminAddressReader, ApiRole.CatalogAdmin, ApiRole.PriceScheduleAdmin, ApiRole.ProductAdmin, ApiRole.ProductAssignmentAdmin, ApiRole.ProductFacetAdmin, ApiRole.SupplierAddressReader } },
            new SecurityProfile() { ID = CustomRole.CustomPromotionAdmin, CustomRoles = new CustomRole[] { CustomRole.CustomPromotionAdmin }, Roles = new ApiRole[] { ApiRole.PromotionAdmin } },
            new SecurityProfile() { ID = CustomRole.ReportAdmin, CustomRoles = new CustomRole[] { CustomRole.ReportAdmin }, Roles = new ApiRole[] { } },
            new SecurityProfile() { ID = CustomRole.ReportReader, CustomRoles = new CustomRole[] { CustomRole.ReportReader }, Roles = new ApiRole[] { } },
            new SecurityProfile() { ID = CustomRole.SellerAdmin, CustomRoles = new CustomRole[] { CustomRole.SellerAdmin }, Roles = new ApiRole[] { ApiRole.AdminUserAdmin } },
            new SecurityProfile() { ID = CustomRole.CustomShipmentAdmin, CustomRoles = new CustomRole[] { CustomRole.CustomShipmentAdmin }, Roles = new ApiRole[] { ApiRole.AddressReader, ApiRole.OrderReader, ApiRole.ShipmentAdmin } },
            new SecurityProfile() { ID = CustomRole.StorefrontAdmin, CustomRoles = new CustomRole[] { CustomRole.StorefrontAdmin }, Roles = new ApiRole[] { ApiRole.ProductFacetAdmin, ApiRole.ProductFacetReader } },
            new SecurityProfile() { ID = CustomRole.CustomSupplierAdmin, CustomRoles = new CustomRole[] { CustomRole.CustomSupplierAdmin }, Roles = new ApiRole[] { ApiRole.SupplierAddressAdmin, ApiRole.SupplierAdmin, ApiRole.SupplierUserAdmin } },
            new SecurityProfile() { ID = CustomRole.CustomSupplierUserGroupAdmin, CustomRoles = new CustomRole[] { CustomRole.CustomSupplierUserGroupAdmin }, Roles = new ApiRole[] { ApiRole.SupplierReader, ApiRole.SupplierUserGroupAdmin } },
			
			// buyer - this is the only role needed for a buyer user to successfully check out
			new SecurityProfile() { ID = CustomRole.BaseBuyer, CustomRoles = new CustomRole[] { CustomRole.BaseBuyer }, Roles = new ApiRole[] { ApiRole.MeAddressAdmin, ApiRole.MeAdmin, ApiRole.MeCreditCardAdmin, ApiRole.MeXpAdmin, ApiRole.ProductFacetReader, ApiRole.Shopper, ApiRole.SupplierAddressReader, ApiRole.SupplierReader } },

			/* these roles don't do much, access to changing location information will be done through middleware calls that
			*  confirm the user is in the location specific access user group. These roles will be assigned to the location 
			*  specific user group and allow us to determine if a user has an admin role for at least one location through 
			*  the users JWT
			*/
			new SecurityProfile() { ID = CustomRole.LocationOrderApprover, CustomRoles = new CustomRole[] { CustomRole.LocationOrderApprover }, Roles = new ApiRole[] { } },
            new SecurityProfile() { ID = CustomRole.LocationViewAllOrders, CustomRoles = new CustomRole[] { CustomRole.LocationViewAllOrders }, Roles = new ApiRole[] { } },
            new SecurityProfile() { ID = CustomRole.LocationAddressAdmin, CustomRoles = new CustomRole[] { CustomRole.LocationAddressAdmin }, Roles = new ApiRole[] { } },
            new SecurityProfile() { ID = CustomRole.LocationPermissionAdmin, CustomRoles = new CustomRole[] { CustomRole.LocationPermissionAdmin }, Roles = new ApiRole[] { } },
            new SecurityProfile() { ID = CustomRole.LocationNeedsApproval, CustomRoles = new CustomRole[] { CustomRole.LocationNeedsApproval }, Roles = new ApiRole[] { } },
            new SecurityProfile() { ID = CustomRole.LocationCreditCardAdmin, CustomRoles = new CustomRole[] { CustomRole.LocationCreditCardAdmin }, Roles = new ApiRole[] { } },
            new SecurityProfile() { ID = CustomRole.LocationResaleCertAdmin, CustomRoles = new CustomRole[] { CustomRole.LocationResaleCertAdmin }, Roles = new ApiRole[] { } },


        };

        public enum CustomRole
        {
            // seller/supplier
            CustomBuyerAdmin,
            BuyerImpersonator,
            CustomCategoryAdmin,
            ContentAdmin,
            CustomMeAdmin,
            MeProductAdmin,
            MeSupplierAddressAdmin,
            MeSupplierAdmin,
            MeSupplierUserAdmin,
            CustomOrderAdmin,
            CustomProductAdmin,
            CustomPromotionAdmin,
            ReportAdmin,
            ReportReader,
            SellerAdmin,
            CustomShipmentAdmin,
            StorefrontAdmin,
            CustomSupplierAdmin,
            CustomSupplierUserGroupAdmin,

            // buyer
            BaseBuyer,
            LocationAddressAdmin,
            LocationOrderApprover,
            LocationViewAllOrders,
            LocationPermissionAdmin,
            LocationNeedsApproval,
            LocationCreditCardAdmin,
            LocationResaleCertAdmin,

            // cms 
            AssetAdmin,
            AssetReader,
            DocumentAdmin,
            DocumentReader,
            SchemaAdmin,
            SchemaReader
        }

        public class SecurityProfile
        {
            public CustomRole ID { get; set; }
            public ApiRole[] Roles { get; set; }
            public CustomRole[] CustomRoles { get; set; }
        }

        public static readonly List<CustomRole> SellerRoles = new List<CustomRole>() {
            CustomRole.CustomBuyerAdmin,
            CustomRole.BuyerImpersonator,
            CustomRole.CustomCategoryAdmin,
            CustomRole.ContentAdmin,
            CustomRole.CustomMeAdmin,
            CustomRole.CustomOrderAdmin,
            CustomRole.CustomProductAdmin,
            CustomRole.CustomPromotionAdmin,
            CustomRole.ReportAdmin,
            CustomRole.ReportReader,
            CustomRole.SellerAdmin,
            CustomRole.CustomShipmentAdmin,
            CustomRole.StorefrontAdmin,
            CustomRole.CustomSupplierAdmin,
            CustomRole.CustomSupplierUserGroupAdmin
        };
    }
}
