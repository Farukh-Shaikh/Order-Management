namespace OrderManagement.Models
{
    public class Cardholder
    {
        public int Id { get; set; }
        public string EncryptedPAN { get; set; }
        public string CardholderName { get; set; }
        public string ExpirationDate { get; set; }
        public string EncryptedCVV { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }

    }
}
