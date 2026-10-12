# CampusEats.Api — Lab 03

Run:
```
dotnet restore
dotnet run
```
Open the printed URL + `/swagger` (e.g. http://localhost:5240/swagger).

## Test sequence for your screenshots

1. `GET /api/menu` → 200, three seeded dishes (Kottu Roti, Fried Rice, Watalappan).
2. `GET /api/menu/1` → 200.
3. `GET /api/menu/99` → 404.
4. `POST /api/menu` with body:
   ```json
   { "name": "Iced Milo", "price": 300, "category": "Drinks" }
   ```
   → 201, response includes `"id": 4`.
5. `PUT /api/menu/4` with body:
   ```json
   { "name": "Iced Milo", "price": 350, "category": "Drinks" }
   ```
   → 204.
6. `DELETE /api/menu/4` → 204.
7. `GET /api/menu/4` again → 404 (confirms delete worked).
8. `POST /api/menu` with an empty `"name": ""` → 400 (automatic model validation from `[ApiController]`).

Take a screenshot of each numbered step in Swagger — these are the screenshots
the Lab 03 Word document needs.
