// ── Slide 12: the SERVICE layer of the menu feature ──────────────
// Only this file knows the URL shapes. Components and hooks call
// these functions – swap json-server for ASP.NET Core later and
// nothing outside this file changes.
import { apiClient } from "../../api/client.js";

export async function fetchMenuItems({ search, category, page, pageSize }) {
  const params = {
    _page: page,
    _limit: pageSize,
    _sort: "name",
  };
  if (search) params.q = search;
  if (category !== "All") params.category = category;

  const res = await apiClient.get("/menuItems", { params });
  return {
    items: res.data,
    // json-server sends the total row count in a header
    totalCount: Number(res.headers["x-total-count"] ?? res.data.length),
  };
}

export async function toggleFavourite(item) {
  const res = await apiClient.patch(`/menuItems/${item.id}`, {
    isFavourite: !item.isFavourite,
  });
  return res.data;
}
