using System;

namespace S3D.ProjectManagement.Exceptions {
    public sealed class ProjectNotOpenedException : Exception {
    }

    public sealed class ProjectAlreadyOpenedException : Exception {
    }

    public sealed class ProjectSyntaxErrorException : Exception {
    }
}
