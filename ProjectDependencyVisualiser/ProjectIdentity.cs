namespace ProjectDependencyVisualiser
{
    public class ProjectIdentity
    {
        protected bool Equals(ProjectIdentity other)
        {
            return string.Equals(Identifier, other.Identifier) && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProjectIdentity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Identifier != null ? Identifier.GetHashCode() : 0)*397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }

        public string Identifier { get; private set; }
        public string Name { get; set; }

        public ProjectIdentity(string identifier, string name)
        {
            Identifier = identifier;
            Name = name;
        }
    }
}