import { createRouter, createWebHistory } from 'vue-router'
import { isAuthenticated } from '@/services/authService'

const routes = [
  // LOGIN (be layout)
  {
    path: '/login',
    name: 'login',
    component: () => import('../pages/Login.vue')
  },

  // VISI puslapiai su header + footer
  {
    path: '/',
    component: () => import('../layouts/DefaultLayout.vue'),
    children: [

      {
        path: '',
        name: 'home',
        component: () => import('../pages/Home.vue')
      },
      {
        path: 'openings',
        name: 'openings',
        component: () => import('../pages/Openings.vue')
      },
      {
        path: '/openings/:openingId',
        name: 'opening-editor',
        component: () => import('@/pages/OpeningEditor.vue')
      },
      {
        path: 'practicing',
        name: 'practicing',
        component: () => import('../pages/Practicing.vue')
      },
      // PUBLIC
      {
        path: 'privacy',
        name: 'privacy',
        component: () => import('../pages/Privacy.vue')
      },
      {
        path: 'terms',
        name: 'terms',
        component: () => import('../pages/Terms.vue')
      },
      {
        path: 'contact',
        name: 'contact',
        component: () => import('../pages/Contact.vue')
      },

      // PROTECTED
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to) => {
  if (to.meta.requiresAuth && !isAuthenticated()) {
    return { name: 'login' }
  }

  if (to.name === 'login' && isAuthenticated()) {
    return { name: 'home' }
  }
})

export default router
