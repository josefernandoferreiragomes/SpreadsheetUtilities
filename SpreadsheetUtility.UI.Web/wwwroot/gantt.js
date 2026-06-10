// helper: wait for element via MutationObserver, then call callback
function waitForElement(selector, callback, maxRetries) {
    maxRetries = maxRetries || 50;
    var el = document.querySelector(selector);
    if (el) {
        callback(el);
        return;
    }
    var attempts = 0;
    var observer = new MutationObserver(function () {
        var target = document.querySelector(selector);
        if (target) {
            observer.disconnect();
            callback(target);
        }
    });
    observer.observe(document.body, { childList: true, subtree: true });
}

//render Gantt chart for tasks
window.renderGanttTasks = (taskDataJson, viewMode, namedLanes) => {
    if (viewMode === undefined) {
        viewMode = 'Week';
    }
    waitForElement("#gantt-chart-tasks", function(container) {
        var tasks = JSON.parse(taskDataJson);
        container.innerHTML = "";

        var chart = new Gantt("#gantt-chart-tasks", tasks, {
            on_click: (task) => console.log(task),
            on_date_change: (task, start, end) => console.log(task, start, end),
            on_progress_change: (task, progress) => console.log(task, progress),
            on_view_change: (mode) => console.log(mode),
            view_mode: viewMode,
            language: 'en',
            lines: 'both',
            today_button: 'true',
            view_mode_select: 'true'
        });

        if (namedLanes == 'true') {
            chart.tasks.forEach(task => {
                task.resource = task.assignedTo;
            });
        }
    });
};

//render Gantt chart for projects
window.renderGanttProjects = (taskDataJson, viewMode, namedLanes) => {
    if (viewMode === undefined) {
        viewMode = 'Week';
    }
    waitForElement("#gantt-chart-projects", function(container) {
        var tasks = JSON.parse(taskDataJson);
        container.innerHTML = "";

        var chart = new Gantt("#gantt-chart-projects", tasks, {
            on_click: (task) => console.log(task),
            on_date_change: (task, start, end) => console.log(task, start, end),
            on_progress_change: (task, progress) => console.log(task, progress),
            on_view_change: (mode) => console.log(mode),
            view_mode: viewMode,
            language: 'en',
            lines: 'both',
            today_button: 'true',
            view_mode_select: 'true'
        });

        if (namedLanes == 'true') {
            chart.tasks.forEach(task => {
                task.resource = task.assignedTo;
            });
        }
    });
};

//render Gantt chart for developer tasks
window.renderDeveloperGanttTasks = (developerTaskDataJson, viewMode, namedLanes) => {
    if (viewMode === undefined) {
        viewMode = 'Week';
    }
    waitForElement("#developer-gantt-chart-tasks", function(container) {
        var tasks = JSON.parse(developerTaskDataJson);
        container.innerHTML = "";

        var chart = new Gantt("#developer-gantt-chart-tasks", tasks, {
            on_click: (task) => console.log(task),
            on_date_change: (task, start, end) => console.log(task, start, end),
            on_progress_change: (task, progress) => console.log(task, progress),
            on_view_change: (mode) => console.log(mode),
            view_mode: viewMode,
            language: 'en',
            lines: 'both',
            today_button: 'true',
            view_mode_select: 'true'
        });

        if (namedLanes == 'true') {
            chart.tasks.forEach(task => {
                task.resource = task.assignedTo;
            });
        }
    });
};
