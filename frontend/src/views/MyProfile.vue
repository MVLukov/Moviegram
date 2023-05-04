<script>
import { useUserStore } from "../stores/user.js";

export default {
  name: "MyProfile",
  setup() {
    const useStore = useUserStore();
    return { useStore };
  },
};
</script>

<template>
  <div class="container mx-auto mt-4 mb-4 p-3 d-flex justify-content-center">
    <div class="card p-4">
      <div
        class="image d-flex flex-column justify-content-center align-items-center"
      >
        <img
          src="https://cdn-icons-png.flaticon.com/512/21/21104.png"
          height="125"
          width="125"
        />
        <span class="name mt-3">{{ useStore.getUserDetails.username }}</span>
        <div
          class="d-flex flex-row justify-content-center align-items-center gap-2"
        >
          <span class="idd1"
            >Email: <b>{{ useStore.getUserDetails.email }}</b></span
          >
          <span><i class="fa fa-copy"></i></span>
        </div>
        <div
          class="d-flex flex-row justify-content-center align-items-center gap-2"
        >
          <span class="idd1"
            >Role: <b>{{ useStore.getUserDetails.roleName }}</b></span
          >
          <span><i class="fa fa-copy"></i></span>
        </div>
        <div class="d-flex mt-2">
          <button class="btn btn-dark p-1 disabled">Edit Profile</button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
* {
  margin: 0;
  padding: 0;
}

body {
  background-color: #000;
}

.card {
  width: 350px;
  background-color: #efefef;
  border: none;
  transition: all 0.5s;
}

.image img {
  transition: all 0.5s;
  background-color: #444444;
  border-radius: 50%;
  padding: 15px;
}

.name {
  font-size: 22px;
  font-weight: bold;
}

.idd {
  font-size: 14px;
  font-weight: 600;
}

.idd1 {
  font-size: 12px;
}

.number {
  font-size: 22px;
  font-weight: bold;
}

.follow {
  font-size: 12px;
  font-weight: 500;
  color: #444444;
}

.btn1 {
  height: 40px;
  width: 150px;
  border: none;
  background-color: #000;
  color: #aeaeae;
  font-size: 15px;
}

.btn1:hover {
  color: #cccccc;
}

.text span {
  font-size: 13px;
  color: #545454;
  font-weight: 500;
}

.icons i {
  font-size: 19px;
}

hr .new1 {
  border: 1px solid;
}

.join {
  font-size: 14px;
  color: #a0a0a0;
  font-weight: bold;
}

.date {
  background-color: #ccc;
}
</style>
