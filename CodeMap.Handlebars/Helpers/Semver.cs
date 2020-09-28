using System;
using System.IO;
using System.Linq;

namespace CodeMap.Handlebars.Helpers
{
    public class Semver : IHandlebarsHelper
    {
        public string Name
            => nameof(Semver);

        public void Apply(TextWriter writer, object context, params object[] parameters)
        {
            var version = parameters.DefaultIfEmpty(context).First() as Version;
            if (version is null)
                throw new ArgumentException("Expected a " + nameof(Version) + " provided as the first parameter or context.");

            writer.Write(version.ToSemver());
        }
    }
}