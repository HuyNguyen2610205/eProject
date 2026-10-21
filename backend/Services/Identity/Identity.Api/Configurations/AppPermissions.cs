namespace Identity.Api.Configurations;

public static class AppPermissions
{
    public static class Users
    {
        public const string View = "Users.View";
        public const string Create = "Users.Create";
        public const string Edit = "Users.Edit";
        public const string Delete = "Users.Delete";
        public const string AssignRole = "Users.AssignRole";
    }

    public static class Roles
    {
        public const string View = "Roles.View";
        public const string Create = "Roles.Create";
        public const string Edit = "Roles.Edit";
        public const string Delete = "Roles.Delete";
    }

    public static class Catalog
    {
        public const string ProductsView = "Catalog.Products.View";
        public const string ProductsCreate = "Catalog.Products.Create";
        public const string ProductsEdit = "Catalog.Products.Edit";
        public const string ProductsDelete = "Catalog.Products.Delete";
        public const string CategoriesManage = "Catalog.Categories.Manage";
    }

    public static class Orders
    {
        public const string View = "Orders.View";
        public const string Create = "Orders.Create";
        public const string Edit = "Orders.Edit";
        public const string Cancel = "Orders.Cancel";
        public const string Approve = "Orders.Approve";
    }

    public static class Reports
    {
        public const string View = "Reports.View";
        public const string Export = "Reports.Export";
    }

    public static class System
    {
        public const string SettingsView = "System.Settings.View";
        public const string SettingsManage = "System.Settings.Manage";
        public const string AuditLogView = "System.AuditLog.View";
    }
}
