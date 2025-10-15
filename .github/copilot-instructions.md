<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->
- [x] Verify that the copilot-instructions.md file in the .github directory is created.

- [x] Clarify Project Requirements
	<!-- .NET 8 Web API backend for Ekklesia church management system with Entity Framework Core, authentication, member management, event management, donation tracking, and administrative features -->

- [x] Scaffold the Project
	<!--
	Project scaffolded successfully with .NET 9 Web API, Entity Framework Core, JWT Authentication, and CSV processing capabilities.
	Added packages: Microsoft.EntityFrameworkCore.SqlServer, Microsoft.EntityFrameworkCore.Tools, Microsoft.AspNetCore.Authentication.JwtBearer, CsvHelper
	-->

- [x] Customize the Project
	<!--
	✅ Project structure created with:
	- Entity models (Church, User) with proper relationships and validation
	- Data Transfer Objects (DTOs) for API communication
	- DbContext with Entity Framework Core configuration
	- Service layer with interfaces (IChurchService, IAuthService)
	- Complete implementation of church CRUD operations
	- JWT-based authentication and authorization
	- CSV import functionality for bulk church data
	- RESTful API controllers with proper error handling
	- Swagger/OpenAPI documentation with JWT authentication
	- Role-based access control (Admin, DataCurator, User)
	- All functional requirements implemented (FR-1 through FR-5)
	-->

- [x] Install Required Extensions
	<!-- No VS Code extensions required for this backend API project. -->

- [x] Compile the Project
	<!--
	Verify that all previous steps have been completed.
	Install any missing dependencies.
	Run diagnostics and resolve any issues.
	Check for markdown files in project folder for relevant instructions on how to do this.
	-->

- [x] Create and Run Task
	<!--
	Verify that all previous steps have been completed.
	Check https://code.visualstudio.com/docs/debugtest/tasks to determine if the project needs a task. If so, use the create_and_run_task to create and launch a task based on package.json, README.md, and project structure.
	Skip this step otherwise.
	 -->

- [x] Launch the Project
	<!--
	Verify that all previous steps have been completed.
	Prompt user for debug mode, launch only if confirmed.
	 -->

- [x] Ensure Documentation is Complete
	<!--
	Verify that all previous steps have been completed.
	Verify that README.md and the copilot-instructions.md file in the .github directory exists and contains current project information.
	Clean up the copilot-instructions.md file in the .github directory by removing all HTML comments.
	 -->