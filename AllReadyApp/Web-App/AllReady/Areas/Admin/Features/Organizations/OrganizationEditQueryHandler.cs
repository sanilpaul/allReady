using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Organization;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationEditQueryHandler : IAsyncRequestHandler<OrganizationEditQuery, OrganizationEditViewModel>
    {
        private readonly AllReadyContext _context;

        public OrganizationEditQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<OrganizationEditViewModel> Handle(OrganizationEditQuery message)
        {
            var organization = await _context.Organizations
                .AsNoTracking()
                .Include(org => org.OrganizationContacts)
                .ThenInclude(orgcontact => orgcontact.Contact)
                .SingleAsync(org => org.Id == message.Id)
                .ConfigureAwait(false);

            var viewModel = new OrganizationEditViewModel
            {
                Id = organization.Id,
                Name = organization.Name,
                Location = organization.Location.ToEditModel(),
                LogoUrl = organization.LogoUrl,
                WebUrl = organization.WebUrl,
                Description = organization.DescriptionHtml,
                Summary =  organization.Summary,
                PrivacyPolicy = organization.PrivacyPolicy,
                PrivacyPolicyUrl = organization.PrivacyPolicyUrl
            };
            var organizationContact = organization.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int) ContactTypes.Primary)?.Contact;
            if (organizationContact == null) return viewModel;

            viewModel.PrimaryContactEmail = organizationContact.Email;
            viewModel.PrimaryContactFirstName = organizationContact.FirstName;
            viewModel.PrimaryContactLastName = organizationContact.LastName;
            viewModel.PrimaryContactPhoneNumber = organizationContact.PhoneNumber;

            return viewModel;
        }
    }
}
