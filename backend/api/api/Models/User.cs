using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace api.Models
{
    [BsonIgnoreExtraElements]//ignorise visak elemenata iz baze --moze da se obrise jer nemamo viska
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]//mongo data type to .net
        public string _id { get; set; }
        [BsonElement("username")]
        public string Username { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }


        [BsonElement("firstName")]
        public string FirstName { get; set; }
        [BsonElement("lastName")]
        public string LastName { get; set; }
        
        public string photoId { get; set; }
        public bool isPermament { get; set; }
        public DateTime dateCreated { get; set; }

    }
}
