using System.Threading.Tasks;
using WisdomPetMedicine.Hospital.Domain.Entities;
using WisdomPetMedicine.Hospital.Domain.Repositories;
using WisdomPetMedicine.Hospital.Domain.ValueObjects;

namespace WisdomPetMedicine.Hospital.Api.Infrastructure
{
    public class PatientAggregateStoreRepository: IPatientAggregateStore
    {
        public async Task SaveAsync(Patient patient)
        {
           await this.petDbContext = petDbContext;
        }
        public async Task<Patient> LoadAsync(PatientId patient)
        {
            return await petDbContext.Pets.FindAsync((Guid)id);
        }

    }
}
