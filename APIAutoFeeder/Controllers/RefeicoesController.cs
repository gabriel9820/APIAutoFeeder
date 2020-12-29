using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using APIAutoFeeder.Models;
using MySql.Data.MySqlClient;

namespace APIAutoFeeder.Controllers
{
    [Authorize]
    public class RefeicoesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Refeicoes/GetAll
        public IQueryable<Refeicao> GetAll(string userId)
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.Refeicoes.Where(x => x.userId == userId);
        }

        // GET: api/Refeicoes/GetById/5
        [ResponseType(typeof(Refeicao))]
        public async Task<IHttpActionResult> GetById(int id)
        {
            Refeicao refeicao = await db.Refeicoes.FindAsync(id);
            if (refeicao == null)
            {
                return NotFound();
            }

            return Ok(refeicao);
        }

        // GET: api/Refeicoes/GetProximaRefeicao
        [ResponseType(typeof(Refeicao))]
        public async Task<IHttpActionResult> GetProximaRefeicao(string userId)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Configuration.LazyLoadingEnabled = false;

                var refeicao = await context.Refeicoes
                               .SqlQuery("SELECT * " +

                                         "FROM tbRefeicao " +

                                         "WHERE userId = @userId AND " +
                                         "horario >= CURRENT_TIME() AND " +
                                         "ativo = true " +

                                         "ORDER BY horario", new MySqlParameter("@userId", userId))
                               .FirstOrDefaultAsync();

                //se não existe nenhuma refeição após CURRENT_TIME(), traz a primeira do próximo dia
                if (refeicao == null)
                {
                    refeicao = await context.Refeicoes
                               .SqlQuery("SELECT * " +

                                         "FROM tbRefeicao " +

                                         "WHERE userId = @userId AND " +
                                         "ativo = true " +

                                         "ORDER BY horario", new MySqlParameter("@userId", userId))
                               .FirstOrDefaultAsync();
                }

                //se não achou nenhuma das duas retorna error 404
                if (refeicao == null)
                {
                    return NotFound();
                }

                return Ok(refeicao);
            }
        }

        // PUT: api/Refeicoes/Put/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(int id, Refeicao refeicao)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != refeicao.id)
            {
                return BadRequest();
            }

            db.Entry(refeicao).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RefeicaoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Refeicoes/Post
        [ResponseType(typeof(Refeicao))]
        public async Task<IHttpActionResult> Post(Refeicao refeicao)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Refeicoes.Add(refeicao);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = refeicao.id }, refeicao);
        }

        // DELETE: api/Refeicoes/Delete/5
        [ResponseType(typeof(Refeicao))]
        public async Task<IHttpActionResult> Delete(int id)
        {
            Refeicao refeicao = await db.Refeicoes.FindAsync(id);
            if (refeicao == null)
            {
                return NotFound();
            }

            db.Refeicoes.Remove(refeicao);
            await db.SaveChangesAsync();

            return Ok(refeicao);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RefeicaoExists(int id)
        {
            return db.Refeicoes.Count(e => e.id == id) > 0;
        }
    }
}