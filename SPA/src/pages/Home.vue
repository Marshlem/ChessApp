<template>
    <section class="min-h-[60vh] grid place-items-center p-6">
        <div class="w-full max-w-sm rounded-xl border border-gray-200 bg-white p-6 space-y-4 shadow-sm">
        <h1 class="text-2xl font-semibold text-gray-900">Welcome to ChessApp</h1>
        <p class="text-gray-600">
            Track your chess games and manage your portfolio with ease.
        </p>
        <div class="mt-4">
            <RouterLink
            to="/login"
            class="rounded-md border border-blue-600 bg-blue-600 px-4 py-2 text-sm font-medium text-white
                     hover:bg-blue-700"
            >
            Get Started
            </RouterLink>
        </div>
        </div>
    </section>
</template>