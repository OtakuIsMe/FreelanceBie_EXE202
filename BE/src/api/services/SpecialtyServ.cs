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
		Task<IActionResult> ViewSpecialties(string? query);
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
					throw new ApplicationException("Failed to add specialty");
				}

				await _cacheService.ClearWithPattern("specialties");

				return SuccessResp.Created("Specialty added successfully");
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}

		public async Task<IActionResult> DeleteSpecialty(Guid id)
		{
			try
			{
				var result = await _specialtyRepo.DeleteSpecialty(id);
				if (!result)
				{
					throw new ApplicationException("Failed to delete specialty");
				}
				return SuccessResp.Ok("Specialty deleted successfully");
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}

		public async Task<IActionResult> UpdateSpecialty(Guid id, SpecialtyCreateDTO specialty)
		{
			try
			{
				var specialtyFinding = await _specialtyRepo.GetSpecialty(id);
				if (specialtyFinding == null)
				{
					throw new ApplicationException("Specialty not found");
				}

				specialtyFinding.Name = specialty.Name ?? specialtyFinding.Name;

				var result = await _specialtyRepo.UpdateSpecialty(specialtyFinding);
				if (!result)
				{
					throw new ApplicationException("Failed to update specialty");
				}
				return SuccessResp.Ok("Specialty updated successfully");
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}

		public async Task<IActionResult> ViewSpecialties(string? query)
		{
			try
			{
				List<Specialty> specialties = null!;
				if (string.IsNullOrEmpty(query))
				{
					specialties = (await _specialtyRepo.GetSpecialties()).Take(6).ToList();
				}
				else
				{
					specialties = await _specialtyRepo.GetSpecialtiesByQuery(query);
				}

				return SuccessResp.Ok(specialties);
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
		}
	}
}
