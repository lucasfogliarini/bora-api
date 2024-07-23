namespace Bora.Entities
{
    public abstract class AccountResponsibility : Entity
    {
        public int AccountId { get; set; }
        public int ResponsibilityId { get; set; }
    }
}
