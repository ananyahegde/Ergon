using AutoMapper;
using Ergon.Models;
using Ergon.DTOs.Role;
using Ergon.DTOs.Branch;
using Ergon.DTOs.Department;
using Ergon.DTOs.Designation;
using Ergon.DTOs.Shift;
using Ergon.DTOs.LeaveType;
using Ergon.DTOs.LeaveEntitlement;
using Ergon.DTOs.LeaveEntitlementComponent;
using Ergon.DTOs.SalaryStructure;
using Ergon.DTOs.SalaryComponent;
using Ergon.DTOs.TaxSlab;
using Ergon.DTOs.PublicHoliday;
using Ergon.DTOs.Country;
using Ergon.DTOs.State;
using Ergon.DTOs.City;
using Ergon.DTOs.Employee;
using Ergon.DTOs.BankAccount;
using Ergon.DTOs.EmployeeDocument;
using Ergon.DTOs.Attendance;
using Ergon.DTOs.Leave;
using Ergon.DTOs.Payroll;
using Ergon.DTOs.PayrollComponent;
using Ergon.DTOs.ReviewCycle;
using Ergon.DTOs.ReviewCycleDetails;
using Ergon.DTOs.Notification;

namespace Ergon.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Role
            CreateMap<CreateRoleRequest, Role>();
            CreateMap<UpdateRoleRequest, Role>();
            CreateMap<Role, RoleResponse>();

            // Branch
            CreateMap<CreateBranchRequest, Branch>();
            CreateMap<UpdateBranchRequest, Branch>();
            CreateMap<Branch, BranchResponse>();

            // Department
            CreateMap<CreateDepartmentRequest, Department>();
            CreateMap<UpdateDepartmentRequest, Department>();
            CreateMap<Department, DepartmentResponse>();

            // Designation
            CreateMap<CreateDesignationRequest, Designation>();
            CreateMap<UpdateDesignationRequest, Designation>();
            CreateMap<Designation, DesignationResponse>();

            // Shift
            CreateMap<CreateShiftRequest, Shift>();
            CreateMap<UpdateShiftRequest, Shift>();
            CreateMap<Shift, ShiftResponse>();

            // LeaveType
            CreateMap<CreateLeaveTypeRequest, LeaveType>();
            CreateMap<UpdateLeaveTypeRequest, LeaveType>();
            CreateMap<LeaveType, LeaveTypeResponse>();

            // LeaveEntitlement
            CreateMap<CreateLeaveEntitlementRequest, LeaveEntitlement>();
            CreateMap<UpdateLeaveEntitlementRequest, LeaveEntitlement>();
            CreateMap<LeaveEntitlement, LeaveEntitlementResponse>();

            // LeaveEntitlementComponent
            CreateMap<CreateLeaveEntitlementComponentRequest, LeaveEntitlementComponent>();
            CreateMap<UpdateLeaveEntitlementComponentRequest, LeaveEntitlementComponent>();
            CreateMap<LeaveEntitlementComponent, LeaveEntitlementComponentResponse>()
                .ForMember(dest => dest.LeaveTypeName, opt => opt.MapFrom(src => src.LeaveType.LeaveTypeName));

            // SalaryStructure
            CreateMap<CreateSalaryStructureRequest, SalaryStructure>();
            CreateMap<UpdateSalaryStructureRequest, SalaryStructure>();
            CreateMap<SalaryStructure, SalaryStructureResponse>();

            // SalaryComponent
            CreateMap<CreateSalaryComponentRequest, SalaryComponent>();
            CreateMap<UpdateSalaryComponentRequest, SalaryComponent>();
            CreateMap<SalaryComponent, SalaryComponentResponse>();

            // TaxSlab
            CreateMap<CreateTaxSlabRequest, TaxSlab>();
            CreateMap<UpdateTaxSlabRequest, TaxSlab>();
            CreateMap<TaxSlab, TaxSlabResponse>();

            // PublicHoliday
            CreateMap<CreatePublicHolidayRequest, PublicHoliday>();
            CreateMap<UpdatePublicHolidayRequest, PublicHoliday>();
            CreateMap<PublicHoliday, PublicHolidayResponse>();

            // Country
            CreateMap<CreateCountryRequest, Country>();
            CreateMap<UpdateCountryRequest, Country>();
            CreateMap<Country, CountryResponse>();

            // State
            CreateMap<CreateStateRequest, State>();
            CreateMap<UpdateStateRequest, State>();
            CreateMap<State, StateResponse>();

            // City
            CreateMap<CreateCityRequest, City>();
            CreateMap<UpdateCityRequest, City>();
            CreateMap<City, CityResponse>();

            // Employee
            CreateMap<CreateEmployeeRequest, Employee>();
            CreateMap<UpdateEmployeeRequest, Employee>();
            CreateMap<Employee, EmployeeListResponse>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.DepartmentName))
                .ForMember(dest => dest.DesignationName, opt => opt.MapFrom(src => src.Designation.DesignationName))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.BranchName))
                .ForMember(dest => dest.EmploymentStatus, opt => opt.MapFrom(src => src.EmploymentStatus.ToString()));
            CreateMap<Employee, EmployeeDetailResponse>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.DepartmentName))
                .ForMember(dest => dest.DesignationName, opt => opt.MapFrom(src => src.Designation.DesignationName))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.BranchName))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
                .ForMember(dest => dest.ShiftName, opt => opt.MapFrom(src => src.Shift.ShiftName))
                .ForMember(dest => dest.SalaryStructureName, opt => opt.MapFrom(src => src.SalaryStructure.SalaryStructureName))
                .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FirstName + " " + src.Manager.LastName : null))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.EmploymentType, opt => opt.MapFrom(src => src.EmploymentType.ToString()))
                .ForMember(dest => dest.EmploymentStatus, opt => opt.MapFrom(src => src.EmploymentStatus.ToString()))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.CityName))
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.State.StateName))
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.CountryName));

            // BankAccount
            CreateMap<BankAccountRequest, BankAccount>();
            CreateMap<BankAccount, BankAccountResponse>();

            // EmployeeDocument
            CreateMap<CreateEmployeeDocumentRequest, EmployeeDocument>();
            CreateMap<EmployeeDocument, EmployeeDocumentResponse>()
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DocumentType.ToString()));

            // Attendance
            CreateMap<CreateAttendanceRequest, Attendance>();
            CreateMap<Attendance, AttendanceResponse>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FirstName + " " + src.Employee.LastName))
                .ForMember(dest => dest.AttendanceStatus, opt => opt.MapFrom(src => src.AttendanceStatus.ToString()));

            // Leave
            CreateMap<CreateLeaveRequest, Leave>();
            CreateMap<Leave, LeaveResponse>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FirstName + " " + src.Employee.LastName))
                .ForMember(dest => dest.LeaveTypeName, opt => opt.MapFrom(src => src.LeaveType.LeaveTypeName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ActionedByName, opt => opt.MapFrom(src => src.ActionedByEmployee != null ? src.ActionedByEmployee.FirstName + " " + src.ActionedByEmployee.LastName : null));

            // Payroll
            CreateMap<CreatePayrollRequest, Payroll>();
            CreateMap<Payroll, PayrollResponse>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FirstName + " " + src.Employee.LastName))
                .ForMember(dest => dest.PayrollStatus, opt => opt.MapFrom(src => src.PayrollStatus.ToString()))
                .ForMember(dest => dest.ApprovedByName, opt => opt.MapFrom(src => src.ApprovedByEmployee != null ? src.ApprovedByEmployee.FirstName + " " + src.ApprovedByEmployee.LastName : null));

            // PayrollComponent
            CreateMap<CreatePayrollComponentRequest, PayrollComponent>();
            CreateMap<UpdatePayrollComponentRequest, PayrollComponent>();
            CreateMap<PayrollComponent, PayrollComponentResponse>()
                .ForMember(dest => dest.PayrollComponentType, opt => opt.MapFrom(src => src.PayrollComponentType.ToString()));

            // ReviewCycle
            CreateMap<CreateReviewCycleRequest, ReviewCycle>();
            CreateMap<UpdateReviewCycleRequest, ReviewCycle>();
            CreateMap<ReviewCycle, ReviewCycleResponse>()
                .ForMember(dest => dest.ReviewCycleStatus, opt => opt.MapFrom(src => src.ReviewCycleStatus.ToString()));

            // ReviewCycleDetails
            CreateMap<CreateReviewCycleDetailsRequest, ReviewCycleDetails>();
            CreateMap<UpdateReviewCycleDetailsRequest, ReviewCycleDetails>();
            CreateMap<ReviewCycleDetails, ReviewCycleDetailsResponse>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FirstName + " " + src.Employee.LastName))
                .ForMember(dest => dest.ReviewCycleName, opt => opt.MapFrom(src => src.ReviewCycle.ReviewName));

            // Notification
            CreateMap<Notification, NotificationResponse>();
        }
    }
}
