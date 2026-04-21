namespace Viamatica.Application.Common;

public static class RoleIds
{
    public const int Administrator = 1;
    public const int Gestor = 2;
    public const int Cashier = 3;
}

public static class RoleNames
{
    public const string Administrator = "Administrador";
    public const string Gestor = "Gestor";
    public const string Cashier = "Cajero";
    public const string AdminOrGestor = $"{Administrator},{Gestor}";
    public const string AnyStaff = $"{Administrator},{Gestor},{Cashier}";
}

public static class UserStatusIds
{
    public const string Active = "ACT";
    public const string Pending = "PEN";
    public const string Inactive = "INA";
}

public static class ContractStatusIds
{
    public const string Active = "ACT";
    public const string Cancelled = "CAN";
    public const string Replaced = "REP";
    public const string Renewed = "REN";
}

public static class AttentionStatusIds
{
    public const int Open = 1;
    public const int Completed = 2;
    public const int Cancelled = 3;
}

public static class AttentionTypeIds
{
    public const string General = "GEN";
    public const string Contract = "CTR";
    public const string Payment = "PAY";
    public const string ChangeService = "CSV";
    public const string ChangePayment = "CFP";
    public const string Cancellation = "CAN";
}

public static class TurnPrefixes
{
    public static string FromAttentionType(string attentionTypeId) => attentionTypeId switch
    {
        AttentionTypeIds.General => "AC",
        AttentionTypeIds.Contract => "CT",
        AttentionTypeIds.Payment => "PS",
        AttentionTypeIds.ChangeService => "CS",
        AttentionTypeIds.ChangePayment => "FP",
        AttentionTypeIds.Cancellation => "CA",
        _ => throw new BusinessRuleException($"No existe prefijo configurado para el tipo de atención {attentionTypeId}.")
    };
}
