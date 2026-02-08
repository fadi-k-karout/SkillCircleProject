using ArgumentNullException = System.ArgumentNullException;

namespace Domain.Models;
public class Skill
{
        public Guid Id { get; set; } 
        public string Name { get; set; } 
        public string Slug { get;  set; }
        public string Description { get; set; }
        public Status Status { get; set; } = new();
        public ICollection<Course> Courses { get;  protected set; } = new List<Course>();
        public ICollection<User> Users { get; protected set; } = new List<User>();
        public Skill(){}
        public Skill(string name, string description, string slug, Status status)
        {
            Name = name;
            Description = description;
            Slug = slug;
            Courses =  new List<Course>();
            Status = status;
        }

        public bool SoftDelete()
        {
            if (Status.IsSoftDeleted) return false;
             
            Status.IsSoftDeleted = true;
            return true;
        }

        public void AddCourse(Course course)
        {
            if(Courses == null)
                throw new ArgumentNullException(nameof(course),"cannot be null");
            
            if (!Courses.Contains(course)) Courses.Add(course);
            
        }

        public void RemoveCourse(Course course)
        {
            if (Courses == null)
                throw new ArgumentNullException(nameof(course), "cannot be null");

            Courses.Remove(course);

        }

        public void AddUser(User user)
        {
            if (Users == null)
                throw new ArgumentNullException(nameof(user),"cannot be null");
            
            if (!Users.Contains(user)) Users.Add(user);
        }

        public void RemoveUser(User user)
        {
            if (Users == null)
                throw new ArgumentNullException(nameof(user),"cannot be null");
            
            Users.Remove(user);
        }

}
