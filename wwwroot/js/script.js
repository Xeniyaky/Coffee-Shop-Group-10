const barCtx = document.getElementById("barChart");

if (barCtx) {
  new Chart(barCtx, {
    type: "bar",
    data: {
      labels: ["Latte", "Cappuccino", "Espresso", "Mocha", "Iced Coffee"],
      datasets: [{
        label: "Best Selling Drinks",
        data: [45, 30, 20, 25, 40],
        borderWidth: 1
      }]
    },
    options: {
      responsive: true,
      plugins: {
        legend: {
          display: true
        }
      }
    }
  });
}

const pieCtx = document.getElementById("pieChart");

if (pieCtx) {
  new Chart(pieCtx, {
    type: "pie",
    data: {
      labels: ["Hot Drinks", "Cold Drinks", "Pastries"],
      datasets: [{
        label: "Order Categories",
        data: [55, 30, 15]
      }]
    },
    options: {
      responsive: true
    }
  });
}

const lineCtx = document.getElementById("lineChart");

if (lineCtx) {
  new Chart(lineCtx, {
    type: "line",
    data: {
      labels: ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"],
      datasets: [{
        label: "Weekly Sales",
        data: [120, 145, 160, 150, 190, 230, 210],
        fill: false,
        tension: 0.2
      }]
    },
    options: {
      responsive: true
    }
  });
}
