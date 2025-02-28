using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.src.api.domains.Database;
using BE.src.api.domains.Model;
using Microsoft.EntityFrameworkCore;

namespace BE.src.api.repositories
{
	public interface ISpecialtyRepo
	{
		Task<List<Specialty>> GetSpecialties();
		Task<bool> AddSpecialty(Specialty specialty);
		Task<bool> UpdateSpecialty(Specialty specialty);
		Task<bool> DeleteSpecialty(Guid id);
		Task<Specialty?> GetSpecialty(Guid id);
		Task<List<Specialty>> GetSpecialtiesByQuery(string query);
		Task<Specialty?> GetSpecialtyByName(string name);
	}
	public class SpecialtyRepo : ISpecialtyRepo
	{
		private readonly FLBDbContext _context;
		public SpecialtyRepo(FLBDbContext context)
		{
			_context = context;
		}
		public async Task<List<Specialty>> GetSpecialties()
		{
			return await _context.Specialties.ToListAsync();
		}

		public async Task<bool> AddSpecialty(Specialty specialty)
		{
			await _context.Specialties.AddAsync(specialty);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> UpdateSpecialty(Specialty specialty)
		{
			_context.Specialties.Update(specialty);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteSpecialty(Guid id)
		{
			var specialty = await _context.Specialties.FirstOrDefaultAsync(x => x.Id == id);
			if (specialty == null)
			{
				return false;
			}
			_context.Specialties.Remove(specialty);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<Specialty?> GetSpecialty(Guid id)
		{
			return await _context.Specialties.FirstOrDefaultAsync(x => x.Id == id);
		}
		public async Task<List<Specialty>> GetSpecialtiesByQuery(string query)
		{
			return await _context.Specialties
						.Where(s => s.Name.Contains(query))
						.OrderBy(s => s.Name.StartsWith(query) ? 0 : 1)
						.ThenBy(s => s.Name)
						.Take(6)
						.ToListAsync();
		}

		public async Task<Specialty?> GetSpecialtyByName(string name)
		{
			return await _context.Specialties.FirstOrDefaultAsync(s => s.Name == name);
		}
	}
}
