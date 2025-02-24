using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.src.api.domains.DTOs.Specialty;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.shared.Type;
using Microsoft.AspNetCore.Mvc;

namespace BE.src.api.services
{
    public interface ISpecialtyServ
    {
        Task<IActionResult> ViewSpecialties();
        Task<IActionResult> AddSpecialty(SpecialtyCreateDTO specialty);
        Task<IActionResult> UpdateSpecialty(Guid id, SpecialtyCreateDTO specialty);
        Task<IActionResult> DeleteSpecialty(Guid id);
    }
	public class SpecialtyServ : ISpecialtyServ
	{
        private readonly ISpecialtyRepo _specialtyRepo;
        private readonly ICacheService _cacheService;
        public SpecialtyServ(ISpecialtyRepo specialtyRepo, ICacheService cacheService)
        {
            _specialtyRepo = specialtyRepo;
            _cacheService = cacheService;
        }

		public async Task<IActionResult> AddSpecialty(SpecialtyCreateDTO specialty)
		{
			try
            {
                var newSpecialty = new Specialty
                {
                    Name = specialty.Name
                };

                var result = await _specialtyRepo.AddSpecialty(newSpecialty);
                if (!result)
                {
                    return ErrorResp.BadRequest("Failed to add specialty");
                }

                await _cacheService.ClearWithPattern("specialties");

                return SuccessResp.Created("Specialty added successfully");
            }
            catch (System.Exception ex)
            {
                return ErrorResp.BadRequest(ex.Message);
            }
		}

		public async Task<IActionResult> DeleteSpecialty(Guid id)
		{
			try
            {
                var result = await _specialtyRepo.DeleteSpecialty(id);
                if (!result)
                {
                    return ErrorResp.BadRequest("Failed to delete specialty");
                }
                return SuccessResp.Ok("Specialty deleted successfully");
            }
            catch (System.Exception ex)
            {
                return ErrorResp.BadRequest(ex.Message);
            }
		}

		public async Task<IActionResult> UpdateSpecialty(Guid id, SpecialtyCreateDTO specialty)
		{
			try
            {
                var specialtyFinding = await _specialtyRepo.GetSpecialty(id);
                if (specialtyFinding == null)
                {
                    return ErrorResp.BadRequest("Specialty not found");
                }

                specialtyFinding.Name = specialty.Name ?? specialtyFinding.Name;

                var result = await _specialtyRepo.UpdateSpecialty(specialtyFinding);
                if (!result)
                {
                    return ErrorResp.BadRequest("Failed to update specialty");
                }
                return SuccessResp.Ok("Specialty updated successfully");
            }
            catch (System.Exception ex)
            {
                return ErrorResp.BadRequest(ex.Message);
            }
		}

		public async Task<IActionResult> ViewSpecialties()
		{
			try
            {
                var key = "specialties";
                var cacheSpecialties = await _cacheService.Get<List<Specialty>>(key);
                if(cacheSpecialties != null)
                    return SuccessResp.Ok(cacheSpecialties);

                var specialties = await _specialtyRepo.GetSpecialties();
                if (specialties.Count == 0)
                {
                    return ErrorResp.BadRequest("No specialties found");
                }

                await _cacheService.Set(key, specialties, TimeSpan.FromMinutes(10));

                return SuccessResp.Ok(specialties);
            }
            catch (System.Exception ex)
            {
                return ErrorResp.BadRequest(ex.Message);
            }
		}
	}
}