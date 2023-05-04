import { createRouter, createWebHistory } from "vue-router";
import { useUserStore } from "../stores/user";

import HomeView from "../views/Home/HomeView.vue";
import axios from "axios";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "home",
      component: HomeView,
      children: [
        {
          path: "/",
          name: "myfeed",
          component: () => import("../views/Home/MyFeed.vue"),
          meta: {
            requireAuth: true,
          },
        },
        {
          path: "/discover",
          name: "discover",
          component: () => import("../views/Home/Discover.vue"),
          meta: {
            requireAuth: true,
          },
        },
        {
          path: "/user/:id",
          name: "userProfile",
          component: () => import("../views/Home/UserProfile.vue"),
          meta: {
            requireAuth: true,
          },
        },
      ],
      meta: {
        requireAuth: true,
      },
    },
    {
      path: "/topRated",
      name: "topRated",
      component: () => import("../views/MoviesViews/TopRated.vue"),
    },
    {
      path: "/mostPopMovies",
      name: "mostPopMovies",
      component: () => import("../views/MoviesViews/MostPopMovies.vue"),
    },
    {
      path: "/mostPopSeries",
      name: "mostPopSeries",
      component: () => import("../views/MoviesViews/MostPopSeries.vue"),
    },
    {
      path: "/topBOlastWeekend",
      name: "topBOlastWeekend",
      component: () => import("../views/MoviesViews/TopBOlastWeekend.vue"),
    },
    {
      path: "/movie/:id",
      name: "movieAbout",
      component: () => import("../views/MoviesViews/MovieAbout.vue"),
    },
    {
      path: "/signup",
      name: "signup",
      component: () => import("../views/Auth/SignUp.vue"),
      meta: {
        requireAuth: false,
        visibleAuthorized: false,
      },
    },
    {
      path: "/signin",
      name: "signin",
      component: () => import("../views/Auth/SignIn.vue"),
      meta: {
        requireAuth: false,
        visibleAuthorized: false,
      },
    },
    {
      path: "/myprofile",
      name: "myProfile",
      component: () => import("../views/MyProfile.vue"),
      meta: {
        requireAuth: true,
      },
    },
    {
      path: "/apiErr",
      name: "apiErr",
      component: () => import("../views/ApiError.vue"),
      meta: {
        requireAuth: false,
      },
    },
  ],
});

router.beforeEach(async (to, from) => {
  const userStore = useUserStore();

  if (to.matched.some((rec) => rec.meta.visibleAuthorized == false)) {
    let result = await axios.get("/api/Users/auth", {
      withCredentials: true,
      validateStatus: () => true,
    });

    if (result.status == 200) {
      userStore.setAuth(true);
      return { name: "home" };
    }

    if (result.status == 401) {
      userStore.setAuth(false);
      return true;
    }

    if (result.status >= 500) {
      return { name: "apiErr" };
    }
  }

  if (to.matched.some((rec) => rec.meta.requireAuth)) {
    let result = await axios.get("/api/Users/auth", {
      withCredentials: true,
      validateStatus: () => true,
    });

    if (result.status == 200) {
      userStore.setAuth(true);
      return true;
    }

    if (result.status == 401) {
      return { name: "signin" };
    }

    if (result.status >= 500) {
      return { name: "apiErr" };
    }
  } else {
    return true;
  }
});

export default router;
