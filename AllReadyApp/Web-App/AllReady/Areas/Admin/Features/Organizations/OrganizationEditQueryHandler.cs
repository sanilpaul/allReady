using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Extensions;
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
                .Include(c => c.Campaigns)
                .Include(l => l.Location)
                .Include(u => u.Users).Include(tc => tc.OrganizationContacts)
                .ThenInclude(c => c.Contact)
                .SingleOrDefaultAsync(org => org.Id == message.Id)
                .ConfigureAwait(false);

            if (organization == null)
            {
                return null;
            }

            var organizationEditViewModel = new OrganizationEditViewModel
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

            if (organization.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact != null)
            {
                organizationEditViewModel = (OrganizationEditViewModel)organization.OrganizationContacts?.SingleOrDefault(tc => tc.ContactType == (int)ContactTypes.Primary)?.Contact.ToEditModel(organizationEditViewModel);
            }
            
            return organizationEditViewModel;
        }
    }
}
